using System;
using Core;
using enemies;
using UnityEditor;
using UnityEditor.EditorTools;
using UnityEngine;

namespace DefaultNamespace
{
    [EditorTool("Waypoint Tool", typeof(IWaypoints))]
    public class WaypointTool : EditorTool
    {
        [SerializeField] private Texture2D m_ToolIcon;

        private static Color[] colors = new[]
        {
            Color.red,
            Color.magenta,
            Color.yellow,
            Color.blue
        };
        
        private GUIContent m_IconContent;
        private Color _color;
        private void OnEnable()
        {
            m_IconContent = new GUIContent()
            {
                image = m_ToolIcon,
                text = "Waypoint Tool",
                tooltip = "Waypoint Tool"
            };
        }


        public override GUIContent toolbarIcon => m_IconContent;

        public override void OnToolGUI(EditorWindow window)
        {
            int cnt = 0;
            foreach (var target in targets)
            {
                var waypoint = target as IWaypoints;
                if (waypoint == null)
                {
                    Debug.LogWarning("OnTOolGUI called without waypoint");
                    continue;
                }
                if(waypoint.WaypointTransforms == null || waypoint.WaypointTransforms.Length == 0)
                    continue;
                var color = colors[cnt];
                cnt++;
                cnt %= colors.Length;
                
                DrawWaypoints(waypoint.WaypointTransforms, color);
            }
        }


        public void DrawWaypoints(Transform[] waypoints, Color color)
        {
            this._color = color;
            for (int i = 0; i < waypoints.Length; i++)
            {
                var last = i == 0 ? waypoints[waypoints.Length - 1] : waypoints[i - 1];
                var cur = waypoints[i];
                using (new Handles.DrawingScope(_color.WithAlpha(0.5f)))
                {
                    Handles.DrawLine(last.position, cur.position);
                }
                DrawWaypointHandle( cur);
            }
        }

        private void DrawWaypointHandle( Transform cur)
        {
            DrawPositionHandle(cur);
            DrawLabels(cur);
        }

        private void DrawPositionHandle(Transform wpTransform)
        {
            EditorGUI.BeginChangeCheck();
            
            Vector3 snap = Vector3.zero;
            var position = wpTransform.position;
            float size = 0.065f * HandleUtility.GetHandleSize(position);
            
            using(new Handles.DrawingScope(this._color))
            {
                Vector3 newPos = Handles.FreeMoveHandle(position, Quaternion.identity, size, snap, Handles.SphereHandleCap);
                newPos.z = position.z;
                position = newPos;
            }
            
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(wpTransform, $"Moved Waypoint {wpTransform.name}");
                wpTransform.position = position;
            }
        }

        private void DrawLabels(Transform cur)
        {
            Color labelColor = Color.Lerp(this._color.WithAlpha(1), Color.black, 0.35f);
            using (new Handles.DrawingScope(labelColor))
            {
                AddWaitLabel(cur);
                Handles.Label(cur.position, cur.name);
            }
        }

        private  void AddWaitLabel(Transform cur)
        {
            var waitpoint = cur.GetComponent<WaitPoint>();
            if (waitpoint != null)
            {
                var labelPosition = cur.position - Vector3.up;
                Handles.Label(labelPosition, $"Wait for {waitpoint.waitTime}s");
            }
        }
    }
}