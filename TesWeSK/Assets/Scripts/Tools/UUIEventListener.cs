using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
public class UUIEventListener : MonoBehaviour,
IPointerClickHandler,
IPointerDownHandler,
IPointerEnterHandler,
IPointerExitHandler,
IPointerUpHandler,
ISelectHandler,
IUpdateSelectedHandler,
IDeselectHandler,
IDragHandler,
IEndDragHandler,
IDropHandler,
IScrollHandler,
IMoveHandler
{
    public delegate void VoidDelegate(UUIEventListener listener);
    public VoidDelegate onClick;
    public VoidDelegate onDown;
    public VoidDelegate onEnter;
    public VoidDelegate onExit;
    public VoidDelegate onUp;
    public VoidDelegate onSelect;
    public VoidDelegate onUpdateSelect;
    public VoidDelegate onDeSelect;
    public VoidDelegate onDrag;
    public VoidDelegate onDragEnd;
    public VoidDelegate onDrop;
    public VoidDelegate onScroll;
    public VoidDelegate onMove;

    //自定义的数据
    public object parameter;
    //底层系统返回的事件数据，根据不同事件，具体类型不同，参见下面事件回调处即可
    private BaseEventData m_pointerEventData;
    public BaseEventData pointerEventData
    {
        get { return m_pointerEventData; }
    }
    //标明所用的音效
    public AudioIDEnum audioEnum;

    public void OnPointerClick(PointerEventData eventData) { if (onClick != null) { m_pointerEventData = eventData; onClick(this); } }
    public void OnPointerDown(PointerEventData eventData) { if (onDown != null) { m_pointerEventData = eventData; onDown(this); } }
    public void OnPointerEnter(PointerEventData eventData) { if (onEnter != null) { m_pointerEventData = eventData; onEnter(this); } }
    public void OnPointerExit(PointerEventData eventData) { if (onExit != null) { m_pointerEventData = eventData; onExit(this); } }
    public void OnPointerUp(PointerEventData eventData) { if (onUp != null) { m_pointerEventData = eventData; onUp(this); } }
    public void OnSelect(BaseEventData eventData) { if (onSelect != null) { m_pointerEventData = eventData; onSelect(this); } }
    public void OnUpdateSelected(BaseEventData eventData) { if (onUpdateSelect != null) { m_pointerEventData = eventData; onUpdateSelect(this); } }
    public void OnDeselect(BaseEventData eventData) { if (onDeSelect != null) { m_pointerEventData = eventData; onDeSelect(this); } }
    public void OnDrag(PointerEventData eventData) { if (onDrag != null) { m_pointerEventData = eventData; onDrag(this); } }
    public void OnEndDrag(PointerEventData eventData) { if (onDragEnd != null) { m_pointerEventData = eventData; onDragEnd(this); } }
    public void OnDrop(PointerEventData eventData) { if (onDrop != null) { m_pointerEventData = eventData; onDrop(this); } }
    public void OnScroll(PointerEventData eventData) { if (onScroll != null) { m_pointerEventData = eventData; onScroll(this); } }
    public void OnMove(AxisEventData eventData) { if (onMove != null) { m_pointerEventData = eventData; onMove(this); } }

    public void Awake()
    {
        UUIPlaySound playSound = GetComponent<UUIPlaySound>();
        if (playSound == null)
            playSound = gameObject.AddComponent<UUIPlaySound>();
        playSound.audioID = audioEnum;
    }

    static public UUIEventListener Get(GameObject go, AudioIDEnum audioID = AudioIDEnum.none)
    {
        UUIEventListener listener = go.GetComponent<UUIEventListener>();
        if (listener == null) listener = go.AddComponent<UUIEventListener>();
        listener.audioEnum = audioID;
        return listener;
    }
}
