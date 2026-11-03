using UnityEngine;

namespace VRLogger
{
    [System.Serializable]
    public class ExperimentConfigData
    {
        public string version;
        public string description;
        public string author;
        public string last_updated;

        public SessionInfo session;
        public ParticipantInfo participants;
        public ExperimentSelection experiment_selection;
        public Modules modules;

        // NEW
        public ParticipantFlow participant_flow;

        // Keep existing fields (compat)
        public EventRoles event_roles;
        public Metrics metrics;
        public CustomEvents custom_events;

        public ExperimentSettings experiment_settings;
        public TrackingSettings tracking_settings;

        [System.Serializable]
        public class SessionInfo
        {
            public string session_name;
            public string group_name;
            public float turn_duration_seconds;
        }

        [System.Serializable]
        public class ParticipantInfo
        {
            public int count;
            public string[] order;
        }

        [System.Serializable]
        public class ExperimentSelection
        {
            public string experiment_id;
            public string formula_profile;
            public string description;
        }

        [System.Serializable]
        public class Modules
        {
            public bool useGazeTracker;
            public bool useMovementTracker;
            public bool useHandTracker;
            public bool useFootTracker;
            public bool useRaycastLogger;
            public bool useCollisionLogger;
        }

        // -------------------------
        // NEW: participant_flow
        // -------------------------
        [System.Serializable]
        public class ParticipantFlow
        {
            // "turns" (default) | "manual"
            public string mode = "turns";

            // "timer" (default) | "gm"
            public string end_condition = "timer";

            public GMControls gm_controls = new GMControls();
        }

        [System.Serializable]
        public class GMControls
        {
            public bool enabled = false;
            public string next_key = "N";
            public string end_key = "E";
            public string pause_key = "P";
            public string force_task_end_key = "T";
        }

        // -------------------------
        // Existing (compat) - leave as is
        // -------------------------
        [System.Serializable]
        public class EventRoles
        {
            public SerializableDictionary<string, string> roles;
        }

        [System.Serializable]
        public class Metrics
        {
            public SerializableDictionary<string, SerializableDictionary<string, string>> definitions;
        }

        [System.Serializable]
        public class CustomEvents
        {
            public SerializableDictionary<string, SerializableDictionary<string, string>> events;
        }

        [System.Serializable]
        public class ExperimentSettings
        {
            public int target_count;
            public float target_speed;
            public int session_time_limit;
            public string difficulty;
            public float turn_duration_seconds;
        }

        [System.Serializable]
        public class TrackingSettings
        {
            public int gaze_sampling_ms;
            public int movement_sampling_ms;
        }
    }
}
