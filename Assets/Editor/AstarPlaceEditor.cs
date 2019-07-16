using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class AstarPlaceEditor : EditorWindow{
    
    
    public static AstarPlaceEditor window;
    private GameObject obj;
    private int width = 0;
    private int height = 0;
    
    [MenuItem("Tools/A Star Place Editor")]
    public static void Init(){

        window = (AstarPlaceEditor)EditorWindow.GetWindow( typeof( AstarPlaceEditor ) );
        
    }

    void OnGUI(){
        
        //GUILayout.Label( "Copy Prefab", EditorStyles.boldLabel );
        
        obj = (GameObject)EditorGUILayout.ObjectField("Copy Prefab", obj, typeof(GameObject), true);

        //AstarTest test = obj.GetComponent<AstarTest>();
        
        
        
        width = EditorGUILayout.IntField( "Width", width );
        height = EditorGUILayout.IntField( "Height", height );

        if( GUILayout.Button( "Run !" ) ){

            if( obj == null ){
                Debug.LogError( "no copy prefab!" );
                return;
            }

            if( width == 0 || height == 0 ){
                Debug.LogError( "Width && Height have to assign the value (not zero). " );
                return;
            }

            float x = obj.transform.localPosition.x;
            float y = obj.transform.localPosition.y;
                
            for( int i = 0; i < height; i++ ){
                for( int j = 0; j < width; j++ ){

                   if( i == 0 && j == 0 ) continue;

                   Instantiate( obj, new Vector3( x + ( j * 1.1f ), y - ( i * 1.1f ), 0 ), Quaternion.identity, obj.transform.parent );
                }
            }
        }
    }
    
    void Update(){
        Repaint();
    }
}
