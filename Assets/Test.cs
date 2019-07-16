using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;

public class Test : MonoBehaviour{

    //public AstarTest manager;
    
    // Start is called before the first frame update
    void Start()
    {
        
        // EventTrigger _trigger = this.GetComponent<EventTrigger>();
        // EventTrigger.Entry _entry = new EventTrigger.Entry();
        // //    要監聽事件的型態 直接重EventTriggerType. 去選擇
        // _entry.eventID = EventTriggerType.PointerClick;
        // _entry.callback = new EventTrigger.TriggerEvent();
        // UnityAction<BaseEventData> call = new UnityAction<BaseEventData>(Click);
        // _entry.callback.AddListener(call);
        // _trigger.triggers.Add(_entry);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    

    public void Click( BaseEventData data){

        PointerEventData d = ( PointerEventData ) data;
        Debug.LogError( d.pointerEnter.name );
    }

   
}
