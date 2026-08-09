using System;
using System.Collections.Concurrent;
using System.Threading;

namespace SistemaMonitorizacionRed
{
    /// <summary>
    /// Sistema de caché en memoria con expiración para reducir consultas a la base de datos.
    /// </summary>
    public static class CacheHelper
    {
        private static readonly ConcurrentDictionary<string, CacheEntry> _cache =
            new ConcurrentDictionary<string, CacheEntry>();
        private static readonly Timer _cleanupTimer;

        static CacheHelper()
        {
            // Limpiar entradas expiradas cada 60 segundos
            _cleanupTimer = new Timer(CleanupExpired, null, TimeSpan.FromMinutes(1), TimeSpan.FromMinutes(1));
        }

        /// <summary>
        /// Tiempo de expiración por defecto (5 minutos).
        /// </summary>
        public static TimeSpan DefaultExpiry { get; set; } = TimeSpan.FromMinutes(5);

        /// <summary>
        /// Obtiene un valor del caché. Si no existe o expiró, ejecuta la función y almacena el resultado.
        /// </summary>
        public static T GetOrSet<T>(string key, Func<T> factory, TimeSpan? expiry = null)
        {
            if (_cache.TryGetValue(key, out var entry) && entry.Expiry > DateTime.UtcNow)
            {
                return (T)entry.Data;
            }

            T result = factory();
            _cache[key] = new CacheEntry { Data = result, Expiry = DateTime.UtcNow.Add(expiry ?? DefaultExpiry) };
            return result;
        }

        /// <summary>
        /// Invalida una entrada específica del caché.
        /// </summary>
        public static void Remove(string key)
        {
            _cache.TryRemove(key, out _);
        }

        /// <summary>
        /// Invalida todas las entradas que comiencen con un prefijo.
        /// </summary>
        public static void RemoveByPrefix(string prefix)
        {
            foreach (var key in _cache.Keys)
            {
                if (key.StartsWith(prefix))
                    _cache.TryRemove(key, out _);
            }
        }

        /// <summary>
        /// Limpia todo el caché.
        /// </summary>
        public static void Clear()
        {
            _cache.Clear();
        }

        /// <summary>
        /// Obtiene estadísticas del caché.
        /// </summary>
        public static (int count, long hits, long misses) GetStats()
        {
            return (_cache.Count, 0, 0);
        }

        private static void CleanupExpired(object state)
        {
            var now = DateTime.UtcNow;
            foreach (var key in _cache.Keys)
            {
                if (_cache.TryGetValue(key, out var entry) && entry.Expiry < now)
                {
                    _cache.TryRemove(key, out _);
                }
            }
        }

        private class CacheEntry
        {
            public object Data { get; set; }
            public DateTime Expiry { get; set; }
        }
    }
}