using UnityEngine;
using Newtonsoft.Json.Linq;
using System;

namespace VRLogger
{
    public class GMConsoleInput : MonoBehaviour
    {
        private bool enabledControls = true; // Default ON

        private KeyCode keyNext = KeyCode.N;
        private KeyCode keyEnd = KeyCode.E;
        private KeyCode keyPause = KeyCode.P;
        private KeyCode keyForceTaskEnd = KeyCode.T;

        // Prevention for double input triggers (Update vs OnGUI)
        private float lastInputTime = 0f;
        private const float INPUT_COOLDOWN = 0.5f;

        private bool AttemptAction()
        {
            if (Time.unscaledTime - lastInputTime >= INPUT_COOLDOWN)
            {
                lastInputTime = Time.unscaledTime;
                return true;
            }
            return false;
        }

        void Start()
        {
            JObject cfg = ExperimentConfig.Instance.GetConfig();
            if (cfg == null) return;

            JObject pf = (JObject)cfg["participant_flow"];
            if (pf == null) return;

            JObject gm = (JObject)pf["gm_controls"];
            if (gm == null) return;

            enabledControls = (bool?)gm["enabled"] ?? false;

            keyNext = ParseKey((string)gm["next_key"], KeyCode.N);
            keyEnd = ParseKey((string)gm["end_key"], KeyCode.E);
            keyPause = ParseKey((string)gm["pause_key"], KeyCode.P);
            keyForceTaskEnd = ParseKey((string)gm["force_task_end_key"], KeyCode.T);

            Debug.Log("[GMConsoleInput] enabled=" + enabledControls
                + " next=" + keyNext + " end=" + keyEnd + " pause=" + keyPause + " force_task_end=" + keyForceTaskEnd);
        }

        void Update()
        {
            if (!enabledControls) return;
            if (ParticipantFlowController.Instance == null) return;
            if (!ParticipantFlowController.Instance.IsRunning()) return;

            if (InputWrapper.GetKeyDown(keyPause) && AttemptAction())
            {
                Debug.Log($"[GMConsoleInput] ⌨️ Detectado KEY PAUSE vía Update: {keyPause}");
                ParticipantFlowController.Instance.TogglePause();
            }

            if (InputWrapper.GetKeyDown(keyEnd) && AttemptAction())
            {
                Debug.Log($"[GMConsoleInput] ⌨️ Detectado KEY END vía Update: {keyEnd}");
                ParticipantFlowController.Instance.GM_EndTurn();
            }

            if (InputWrapper.GetKeyDown(keyNext) && AttemptAction())
            {
                Debug.Log($"[GMConsoleInput] ⌨️ Detectado KEY NEXT vía Update: {keyNext}");
                ParticipantFlowController.Instance.GM_NextParticipant();
            }

            if (InputWrapper.GetKeyDown(keyForceTaskEnd) && AttemptAction())
            {
                Debug.Log($"[GMConsoleInput] ⌨️ Detectado KEY FORCE TASK END vía Update: {keyForceTaskEnd}");
                LogForcedTaskEnd();
            }
        }

        // Método de seguridad: OnGUI puede capturar teclas a veces incluso cuando el sistema de Input
        // principal las ignora (ej. problemas de foco en el editor o conflictos de InputSystem)
        void OnGUI()
        {
            if (!enabledControls) return;
            if (Event.current != null && Event.current.type == EventType.KeyDown)
            {
                KeyCode pressed = Event.current.keyCode;
                if (pressed == KeyCode.None) return;

                if (ParticipantFlowController.Instance != null && ParticipantFlowController.Instance.IsRunning())
                {
                    if (pressed == keyPause && AttemptAction())
                    {
                        Debug.Log($"[GMConsoleInput] ⌨️ Detectado PAUSE vía OnGUI: {keyPause}");
                        ParticipantFlowController.Instance.TogglePause();
                        Event.current.Use();
                    }
                    else if (pressed == keyEnd && AttemptAction())
                    {
                        Debug.Log($"[GMConsoleInput] ⌨️ Detectado END vía OnGUI: {keyEnd}");
                        ParticipantFlowController.Instance.GM_EndTurn();
                        Event.current.Use();
                    }
                    else if (pressed == keyNext && AttemptAction())
                    {
                        Debug.Log($"[GMConsoleInput] ⌨️ Detectado NEXT vía OnGUI: {keyNext}");
                        ParticipantFlowController.Instance.GM_NextParticipant();
                        Event.current.Use();
                    }
                    else if (pressed == keyForceTaskEnd && AttemptAction())
                    {
                        Debug.Log($"[GMConsoleInput] ⌨️ Detectado FORCE TASK END vía OnGUI: {keyForceTaskEnd}");
                        LogForcedTaskEnd();
                        Event.current.Use();
                    }
                }
            }
        }

        static void LogForcedTaskEnd()
        {
            try
            {
                _ = LoggerService.LogEvent("task_end", "task_end", "gm_abandoned", new { reason = "gm_force_task_end" });
            }
            catch (Exception ex)
            {
                Debug.LogWarning("[GMConsoleInput] LogForcedTaskEnd: " + ex.Message);
            }
        }

        private KeyCode ParseKey(string s, KeyCode fallback)
        {
            if (string.IsNullOrEmpty(s)) return fallback;
            try
            {
                return (KeyCode)Enum.Parse(typeof(KeyCode), s, true);
            }
            catch
            {
                return fallback;
            }
        }
    }
}
