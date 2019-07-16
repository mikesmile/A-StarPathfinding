using System.Collections;
using System.Collections.Generic;
using UnityEditor.PackageManager;
using UnityEngine;
using UnityEngine.EventSystems;

public class AstarTest : MonoBehaviour{

    public List<GameObject> places = new List<GameObject>();

    private List<NodeData> openList = new List<NodeData>();
    private List<NodeData> closeList = new List<NodeData>();
    private List<NodeData> currentNeighbour = new List<NodeData>();
    
    private List<NodeData> rightRoad = new List<NodeData>();
    // private int[] f_score;
    // private int[] g_score;
    // private int[] h_score;
    
    private GameObject startNode;
    private GameObject targetNode;
    private GameObject currentNode;
    private int startIndex;
    private int targetIndex;
    private int currentIndex;
    private bool flag = false;

    private int count = 0;
    void Start(){
       
        // f_score = new int[places.Count];
        // g_score = new int[places.Count];
        // h_score = new int[places.Count];

        //places[0].GetComponent<MeshRenderer>().material.color = Color.blue;

        // Debug.LogError( places[0].GetComponent<MeshRenderer>().material.color );
        // Debug.LogError( places[1].GetComponent<MeshRenderer>().material.color );
        //
        // if(places[0].GetComponent<MeshRenderer>().material.color == Color.white)
        //     Debug.LogError( "Yes" );
        //
        // if(places[1].GetComponent<MeshRenderer>().material.color == Color.black)
        //     Debug.LogError( "Yes" );

    }

    void OnGUI(){

        GUIStyle labelStyle = new GUIStyle();
        labelStyle.fontStyle = FontStyle.Bold;
        labelStyle.fontSize = 15;
        
        GUI.Label( new Rect(20,20,100,30),"1. Click Start Node", labelStyle );
       
        if( startNode != null ){
            
            GUI.Label( new Rect(20,70,100,30),"OK Done.", labelStyle );
            
            GUI.Label( new Rect(20,120,100,30),"2. Click Target Node", labelStyle );
        }

        if( targetNode != null ){
            
            GUI.Label( new Rect(20,170,100,30),"OK Done.", labelStyle );

            if( GUI.Button( new Rect( 20, 220, 150, 30 ), "Start serach path." ) ){

                StartCoroutine( AstarSearchPath() );
                //AstarSearchPath();

            }
        }
        
        
    }


    public void Click( BaseEventData data){

        PointerEventData d = ( PointerEventData ) data;
        //Debug.LogError( d.pointerEnter.name );


        for( int i = 0; i < places.Count; i++ ){
            
            if( places[ i ] == d.pointerEnter ){
                

                if( startNode == null ){

                    startNode = places[ i ];
                    startIndex = i;
                    places[ i ].GetComponent<MeshRenderer>().material.color = Color.blue;
                    
                    return;
                }
                
                if( targetNode == null ){

                    targetNode = places[ i ];
                    targetIndex = i;
                    places[ i ].GetComponent<MeshRenderer>().material.color = Color.blue;
                    
                    return;
                }
            }
        }
    }


    IEnumerator AstarSearchPath(){

        openList.Add( new NodeData( startNode,startIndex,0 ) );

        //openList[0].GetComponent<MeshRenderer>().material.color = Color.red;
        
        while( currentNode != targetNode ){
            
            //Debug.LogError( "Test " + count );
            if( openList.Count == 1 ){
                currentNode = openList[ 0 ].obj;
                currentIndex = startIndex;
                openList.RemoveAt( 0 );

                closeList.Add( new NodeData( currentNode,currentIndex,0 ) );//起點不需要父親

            }
            else{

                int lowestIndex = 0;//起始最低
                for( int i = 0; i < openList.Count; i++ ){

                    if( openList[ lowestIndex ].f_score > openList[ i ].f_score )
                        lowestIndex = i;
                }
                
                closeList.Add( openList[lowestIndex] );

                if( openList[ lowestIndex ].obj != targetNode )
                    closeList[ closeList.Count - 1 ].obj.GetComponent<MeshRenderer>().material.color = Color.red;
                
                currentNode = openList[ lowestIndex ].obj;
                currentIndex = openList[ lowestIndex ].placeIndex;

                //Debug.LogError( currentIndex + " count:"+count );
                openList.RemoveAt( lowestIndex );
                
                //yield return new WaitForSeconds(0.05f);
            }

            if( currentNode == targetNode ){
                Debug.LogError( "Find the target!" );
                
                FindRightRoad( closeList[closeList.Count - 1].parentIndex );
                
                for( int i = rightRoad.Count -1; i >= 0; i-- ){
                    rightRoad[i].obj.GetComponent<MeshRenderer>().material.color = Color.blue;
                    yield return new WaitForSeconds(0.01f);
                }
                
                yield break;
            }

            NeighbourOperation();

            foreach( var nodeData in currentNeighbour ){

                if( nodeData.obj.GetComponent<MeshRenderer>().material.color == Color.black || IsContainCloseList(nodeData) )
                    continue;

                if( !IsContainOpenList( nodeData ) ){

                    //set Parent
                    nodeData.parentIndex = closeList[ closeList.Count - 1 ].placeIndex;
                    //Debug.LogError( "Index: "+nodeData.placeIndex + " parentIndex: "+nodeData.parentIndex );
                    //ser f value
                    set_Fvalue( nodeData, closeList[ closeList.Count - 1 ] );
                    if( nodeData.obj != targetNode )
                        nodeData.obj.GetComponent<MeshRenderer>().material.color = Color.yellow;
                    //add new node to openList
                    openList.Add( nodeData );
                    //Debug.LogError( currentIndex+ " 新增:"+nodeData.placeIndex );

                }

                if( IsShorterInNewPath( nodeData, closeList[ closeList.Count - 1 ] ) ){
                    //set new f value
                    nodeData.f_score = nodeData.g_score + nodeData.h_score;
                    //set new Parent
                    nodeData.parentIndex = closeList[ closeList.Count - 1 ].placeIndex;
                    //update to openList
                    for( int i = 0; i < openList.Count; i++ ){
                        if( openList[ i ].placeIndex == nodeData.placeIndex ){
                            openList[ i ] = nodeData;
                            break;
                        }
                    }
                }
                
            }
            
            yield return new WaitForSeconds(0.01f);
        }

    }

    private bool IsContainCloseList( NodeData nodeData ){

        for( int i = 0; i < closeList.Count; i++ ){
            if( closeList[ i ].obj == nodeData.obj )
                return true;
        }

        return false;
    }

    private bool IsContainOpenList( NodeData nodeData ){
        
        for( int i = 0; i < openList.Count; i++ ){
            if( openList[ i ].obj == nodeData.obj )
                return true;
        }

        return false;
    }

    /// <summary>
    /// 只做是否符合範圍的判定(超出格子)
    /// </summary>
    private void NeighbourOperation(){

        currentNeighbour.Clear();//清掉重撈一次
        
        for( int i = 0; i < 3; i++ ){
            if( currentIndex - 26 + i < 0 ) continue;
            if( i == 0 && (currentIndex - 26 + i)%25 == 24) continue;
            if( i == 2 && (currentIndex - 26 + i)%25 == 0) continue;
                
           currentNeighbour.Add( new NodeData( places[currentIndex - 26 + i], (currentIndex - 26 + i), currentIndex ) );
        }

        if( currentIndex + 1 >= 0 && !((currentIndex+1)%25 == 0) )
            currentNeighbour.Add( new NodeData( places[ currentIndex + 1 ], ( currentIndex + 1 ), currentIndex ) );
        
        if( currentIndex - 1 >= 0 && !((currentIndex-1)%25 == 24))
            currentNeighbour.Add( new NodeData( places[ currentIndex - 1 ], ( currentIndex - 1 ), currentIndex ) );

        for( int i = 0; i < 3; i++ ){
            if( currentIndex + 24 + i > places.Count-1 ) continue;
            if( i == 0 && (currentIndex + 24 + i)%25 == 24) continue;
            if( i == 2 && (currentIndex + 24 + i)%25 == 0) continue;

            currentNeighbour.Add( new NodeData( places[currentIndex + 24 + i], (currentIndex + 24 + i), currentIndex ) );
        }
    }

    public bool IsShorterInNewPath( NodeData oldOpenNode, NodeData currentNode ){

        
        if( !IsContainOpenList(oldOpenNode) ) return false; //必須確定此結點已經在openList裡面(才可以比較g值)，否則返回false

        int g_oldScore = 0;
        
        for( int i = 0; i < openList.Count; i++ ){

            if( openList[ i ].obj == oldOpenNode.obj ){
                g_oldScore = openList[ i ].g_score;
                break;
            }
        }

        
        int diffIndex = Mathf.Abs( oldOpenNode.placeIndex - currentNode.placeIndex );

        if( diffIndex == 1 || diffIndex == 25 ){

            if( g_oldScore > currentNode.g_score + 10 ){

                oldOpenNode.g_score = currentNode.g_score + 10;
                return true;
            }
            else
                return false;

        }
        else{

            if( g_oldScore > currentNode.g_score + 14 ){

                oldOpenNode.g_score = currentNode.g_score + 14;
                return true;
            }
            else
                return false;

        }
    }

    public void set_Fvalue( NodeData nodeData, NodeData parentNodeData ){

        int nodeIndex = nodeData.placeIndex;
        int diffIndex = Mathf.Abs( nodeIndex - startIndex );

        nodeData.g_score = set_Gvalue( nodeData, parentNodeData );
        //Debug.LogError( nodeData.g_score );
        nodeData.h_score = set_Hvalue( nodeIndex, targetIndex );//評估值(切比雪夫距離)

        nodeData.f_score = nodeData.g_score + nodeData.h_score;

    }

    public int set_Gvalue( NodeData nodeData, NodeData parentNode ){

        int addValue = 0;
        int diff = Mathf.Abs( nodeData.placeIndex - parentNode.placeIndex );

        if( diff == 1 || diff % 25 == 0 )
            addValue = 10;
        else
            addValue = 14;

        return parentNode.g_score + addValue;

    }
    
    public int set_Hvalue( int currentIndex, int goalIndex, int slashCount = 0 ){
       
        int diff = Mathf.Abs( currentIndex - goalIndex );
        
        if( currentIndex/25 == goalIndex/25 ){

            return diff * 10 + slashCount * 14;
        }
        else{

            if( diff % 25 == 0 ){

                return ( diff / 25 ) * 10 + slashCount * 14;
            }
            else{

                // int div = diff - ( diff % 25 );
                int div = 0;
                if( currentIndex > goalIndex )
                    div = goalIndex + ( ( diff / 25 ) * 25 );
                else
                    div = goalIndex - ( ( diff / 25 ) * 25 );
                
                
                bool IsRightHand = false;
                
                if( currentIndex > goalIndex ){

                    if( currentIndex/25 == div/25 ){ //範圍內同一行(不加1)

                        IsRightHand = ( goalIndex + ( ( diff / 25 ) * 25 ) ) > currentIndex;
                        //Debug.LogError( "1" );
                    }
                    else{ //範圍外(加1)

                        IsRightHand = ( goalIndex + ( ( ( diff / 25 ) + 1 ) * 25 ) ) > currentIndex;
                        //Debug.LogError( "2" );
                    }        
                }
                else{

                    if( currentIndex/25 == div/25 ){ //範圍內同一行(不加1)

                        IsRightHand = ( goalIndex - ( ( diff / 25 ) * 25 ) ) > currentIndex;
                        //Debug.LogError( "3" );
                    }
                    else{ //範圍外(加1)

                        IsRightHand = ( goalIndex - ( ( ( diff / 25 ) + 1 ) * 25 ) ) > currentIndex;
                        //Debug.LogError( "4" );
                    }

                }

                
                if( IsRightHand ){ //右邊

                    if( currentIndex > goalIndex ){ //右上 index比較小故在上面
                        slashCount++;
                        return set_Hvalue( currentIndex - 24, goalIndex, slashCount );
                    }
                    else{ //右下
                        slashCount++;
                        return set_Hvalue( currentIndex + 26, goalIndex, slashCount );
                    }
                }
                else{ //左邊
                    
                    if( currentIndex > goalIndex ){ //左上 index比較小故在上面
                        
                        slashCount++;
                        return set_Hvalue( currentIndex - 26, goalIndex, slashCount );
                    }
                    else{ //左下
                        
                        slashCount++;
                        return set_Hvalue( currentIndex + 24, goalIndex, slashCount );
                    }
                }
            }
        }
        
    }

    public void FindRightRoad( int parentIndex ){
        
        while( parentIndex != startIndex ){

            for( int i = 0; i < closeList.Count; i++ ){

                if( parentIndex == closeList[ i ].placeIndex ){

                    rightRoad.Add( closeList[ i ] );
                    parentIndex = closeList[ i ].parentIndex;
                    break;
                }
            }
        }

        //Debug.LogError( rightRoad.Count );
    }
}

public class NodeData{

    public GameObject obj;
    public int f_score;
    public int g_score;
    public int h_score;

    public int placeIndex; //實際index
    public int parentIndex; //該指向哪一個index

    public NodeData( GameObject obj, int placeIndex, int parentIndex){

        this.obj = obj;
        this.placeIndex = placeIndex;
        this.parentIndex = parentIndex;
        
        f_score = 0;
        g_score = 0;
        h_score = 0;
    }

}
