using UnityEngine.UIElements;
using UnityEngine;

public class UITesting : MonoBehaviour
{
    public UIDocument doc;
    private Vector2 dragStartPos;

    private void Start()
    {
        var root = doc.rootVisualElement;
        var drag = root.Q("red-draggable");

        drag.RegisterCallback<MouseDownEvent>(e =>
        {
            e.target.CaptureMouse();
            dragStartPos = e.localMousePosition;
        });

        drag.RegisterCallback<MouseMoveEvent>(e =>
        {
            if (!e.target.HasMouseCapture())
                return;

            Vector2 delta = e.localMousePosition - dragStartPos;
            drag.style.left = drag.layout.x + delta.x;
            drag.style.top = drag.layout.y + delta.y;
        });

        drag.RegisterCallback<MouseUpEvent>(e =>
        {
            e.target.ReleaseMouse();
        });

        var area = root.Q("area");
        area.RegisterCallback<MouseEnterEvent>(e => Debug.Log("Enter Area"));
    }
}
