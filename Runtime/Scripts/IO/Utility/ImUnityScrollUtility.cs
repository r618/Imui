#if UNITY_WEBGL && !UNITY_EDITOR
#define WEBGL_BUILD
#endif

using UnityEngine;

#if ENABLE_INPUT_SYSTEM
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.UI;
#endif

namespace Imui.IO.Utility
{
    public static class ImUnityScrollUtility
    {
        public static Vector2 ProcessScrollDelta(float dx, float dy)
        {
#if ENABLE_INPUT_SYSTEM || WEBGL_BUILD
            dx = -dx;
#endif

#if ENABLE_INPUT_SYSTEM
            var eventSystem = EventSystem.current?.currentInputModule as InputSystemUIInputModule;
            if (eventSystem)
            {
                dx /= eventSystem.scrollDeltaPerTick;
                dy /= eventSystem.scrollDeltaPerTick;
            }
#endif

            return new Vector2(dx, dy);
        }
    }
}