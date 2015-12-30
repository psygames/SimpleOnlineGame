using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
public class UUIPlaySound : MonoBehaviour, IPointerClickHandler
{
    public AudioIDEnum audioID;

    //public void PlayClick()
    //{

    //}
    public void OnPointerClick(PointerEventData eventData)
    {
        if (audioID != AudioIDEnum.none)
        {
            //TableAudio tableAudio = TableManager.instance.GetPropertiesById<TableAudio>((int)audioID);
            //AudioClip clip = ResourceManager.instance.GetResourceByPath<AudioClip>(tableAudio.path);
            //AudioManager.instance.PlayUIAudio((int)audioID);
        }
    }
}
