using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIHelper
{
    public static void HideTransform(Transform trans)
    {
        trans.localPosition = Vector3.one * 100000f;
    }

    public static void ShowTransform(Transform trans)
    {
        trans.localPosition = Vector3.zero;
    }

    public static Vector2 GetListenerPos(UUIEventListener listener)
    {
        return ((PointerEventData)listener.pointerEventData).position;
    }

    public static Vector2 GetListenerDelta(UUIEventListener listener)
    {
        return ((PointerEventData)listener.pointerEventData).delta;
    }

    public static void SetImageAlpha(Image image,float a)
    {
        image.color = new Color(image.color.r, image.color.g, image.color.b, a);
    }

    public static void SetImageAlpha(Text image, float a)
    {
        image.color = new Color(image.color.r, image.color.g, image.color.b, a);
    }
}
