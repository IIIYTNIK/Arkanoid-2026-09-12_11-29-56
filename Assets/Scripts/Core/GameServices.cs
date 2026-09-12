using System;
using System.Collections.Generic;

namespace Arkanoid.Core
{
    /// <summary>
    /// Простой Service Locator для регистрации и получения игровых систем по интерфейсу.
    /// Не является полноценным DI-контейнером — сознательный выбор для MVP с 2 разработчиками.
    /// Правило команды: доступ к менеджерам ТОЛЬКО через интерфейсы этого класса.
    /// Прямые ссылки на конкретные реализации (LifeManager.Instance и т.п.) запрещены
    /// вне Assets/Scripts/Core.
    /// </summary>
    public static class GameServices
    {
        private static readonly Dictionary<Type, object> Services = new();

        public static void Register<TService>(TService implementation) where TService : class
        {
            var type = typeof(TService);
            if (Services.ContainsKey(type))
            {
                throw new InvalidOperationException(
                    $"Service {type.Name} is already registered. " +
                    "Call Reset() before re-registering (e.g. on scene reload).");
            }

            Services[type] = implementation;
        }

        public static TService Get<TService>() where TService : class
        {
            var type = typeof(TService);
            if (!Services.TryGetValue(type, out var service))
            {
                throw new InvalidOperationException(
                    $"Service {type.Name} is not registered. " +
                    "Ensure GameManager.Awake() has run and registered all core services.");
            }

            return (TService)service;
        }

        public static bool TryGet<TService>(out TService service) where TService : class
        {
            if (Services.TryGetValue(typeof(TService), out var raw))
            {
                service = (TService)raw;
                return true;
            }

            service = null;
            return false;
        }

        /// <summary>
        /// Очищает реестр. Обязательно вызывать при Restart/переходе в MainMenu,
        /// иначе повторная регистрация в новом GameManager.Awake() бросит исключение.
        /// </summary>
        public static void Reset()
        {
            Services.Clear();
        }
    }
}
