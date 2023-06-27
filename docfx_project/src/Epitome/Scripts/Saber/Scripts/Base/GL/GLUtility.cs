using UnityEngine;
//脚本作者:Saber

public static class GLUtility 
{
    public  const float lineOffest = 0.002f;
    /// <summary>
    /// GL绘制线,但更粗
    /// </summary>
    /// <param name="from"></param>
    /// <param name="to"></param>
    /// <param name="size"></param>
    [System.Diagnostics.Conditional("UNITY_EDITOR")]
    public static void DrawLine(Vector3 from,Vector3 to,int size=10)
    {
#if UNITY_EDITOR
        from = new Vector3(from.x - lineOffest * size, from.y, from.z - lineOffest * size);
        to = new Vector3(to.x - lineOffest * size, to.y, to.z - lineOffest * size);
        for (int i = 0; i < 1 + size * 2; i++)
        {
            Gizmos.DrawLine(from, to);
            from.x += lineOffest;
            from.z += lineOffest;
            to.x += lineOffest;
            to.z += lineOffest;
        }
#endif
    }
    /// <summary>
    /// Draw a circular arc in 3D space.
    /// </summary>
    /// <param name="center">The center of the circle.</param>
    /// <param name="normal">The normal of the circle.</param>
    /// <param name="from">The direction of the point on the circle circumference, relative to the center, where the arc begins.</param>
    /// <param name="angle"> The angle of the arc, in degrees.</param>
    /// <param name="radius">The radius of the circle Note: Use HandleUtility.GetHandleSize where you might want to have constant screen-sized handles.</param>
    [System.Diagnostics.Conditional("UNITY_EDITOR")]
    public static void DrawWireArc(Vector3 center, Vector3 normal, Vector3 from, float angle, float radius)
    {
#if UNITY_EDITOR
        UnityEditor.Handles.color = Gizmos.color;
        UnityEditor.Handles.DrawWireArc(center, normal, from, angle, radius);
#endif
    }

    /// <summary>
    /// Draw a circular sector (pie piece) in 3D space.
    /// </summary>
    /// <param name="center">The center of the circle.</param>
    /// <param name="normal">The normal of the circle.</param>
    /// <param name="from">The direction of the point on the circle circumference, relative to the center, where the arc begins.</param>
    /// <param name="angle"> The angle of the arc, in degrees.</param>
    /// <param name="radius">The radius of the circle Note: Use HandleUtility.GetHandleSize where you might want to have constant screen-sized handles.</param>
    [System.Diagnostics.Conditional("UNITY_EDITOR")]
    public static void DrawSolidArc(Vector3 center, Vector3 normal, Vector3 from, float angle, float radius)
    {
#if UNITY_EDITOR
        UnityEditor.Handles.color = Gizmos.color;
        UnityEditor.Handles.DrawSolidArc(center, normal, from, angle, radius);
#endif
    }

    /// <summary>
    /// Draw the outline of a flat disc in 3D space.
    /// </summary>
    /// <param name="center">The center of the disc.</param>
    /// <param name="normal">The normal of the disc.</param>
    /// <param name="radius">The radius of the disc.</param>
    [System.Diagnostics.Conditional("UNITY_EDITOR")]
    public static void DrawWireDisc(Vector3 center, Vector3 normal, float radius)
    {
#if UNITY_EDITOR
        UnityEditor.Handles.color = Gizmos.color;
        UnityEditor.Handles.DrawWireDisc(center, normal, radius);
#endif
    }

    /// <summary>
    /// Draw a solid flat disc in 3D space.
    /// Note: Use HandleUtility.GetHandleSize where you might want to have constant screen-sized handles.
    /// </summary>
    /// <param name="center">The center of the disc.</param>
    /// <param name="normal">The normal of the disc.</param>
    /// <param name="radius">The radius of the disc.</param>
    [System.Diagnostics.Conditional("UNITY_EDITOR")]
    public static void DrawSolidDisc(Vector3 center, Vector3 normal, float radius)
    {
#if UNITY_EDITOR
        UnityEditor.Handles.color = Gizmos.color;
        UnityEditor.Handles.DrawSolidDisc(center, normal, radius);
#endif
    }

    /// <summary>
    /// Make a text label positioned in 3D space.
    /// </summary>
    /// <param name="position">Position in 3D space as seen from the current handle camera.</param>
    /// <param name="text">Text to display on the label.</param>
    [System.Diagnostics.Conditional("UNITY_EDITOR")]
    public static void Label(Vector3 position, string text)
    {
#if UNITY_EDITOR
        UnityEditor.Handles.color = Gizmos.color;
        UnityEditor.Handles.Label(position, text);
#endif
    }

    /// <summary>
    /// Make a text label positioned in 3D space.
    ///  Note: Use HandleUtility.GetHandleSize where you might want to have constant screen-sized handles.
    /// </summary>
    /// <param name="position">Position in 3D space as seen from the current handle camera.</param>
    /// <param name="content">Text, image and tooltip for this label.</param>
    /// <param name="style">The style to use. If left out, the label style from the current GUISkin is used.</param>
    [System.Diagnostics.Conditional("UNITY_EDITOR")]
    public static void Label(Vector3 position, GUIContent content, GUIStyle style)
    {
#if UNITY_EDITOR
        UnityEditor.Handles.color = Gizmos.color;
        UnityEditor.Handles.Label(position, content, style);
#endif
    }
}

