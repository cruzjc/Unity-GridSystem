using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

//https://unity3d.com/learn/tutorials/topics/navigation/baking-navmesh-runtime
//https://github.com/Unity-Technologies/NavMeshComponents

public class GridSystem : MonoBehaviour {
    //the number of grid units created
    public int numberOfGridUnits;



    public bool useNavMesh;
    private NavMeshSurface[,,] surfaces;//surfaces to generate a navmesh for


    #region Initialization Stuff"
    [Header("Initialization Stuff")]
    //grid size
    public int columns;
    public int rows;
    public int height;//keep at one for a single layer grid
    //scale the grid unit
    public float scaleX;
    public float scaleY;
    public float scaleZ;


    public bool useGameObject;//whether to use a prefab gridunit or a more abstract system
    public bool visualizeGrid;
    public bool destroyAfterCreation;//destroys script after creating grid, disable to keep variables on scene

    //a single plane with a grid material to help visualize a grid not using a gameobject grid unit 
    private GameObject planeGrid;//might not even need this
    public GameObject gridUnit;//use a square 1x1 object


    private GameObject[,,] gridObjects;
    private Vector3[,,] grid;//instead of an array of game objects, this is an array of vectors3 to store positions
    #endregion

    private void Awake()
    {
        createGrid();
        if(destroyAfterCreation)Destroy(this);//removes script instance (for easy prefabbing)
        if (useNavMesh) buildNavmesh();
    }

    // Use this for initialization
    void Start() {


	}
	
	// Update is called once per frame
	void Update () {
    }

    //creates either a grid of objects or an abstract grid
    public void createGrid(){
        Vector3 originalRotation = this.transform.eulerAngles;
        this.transform.eulerAngles = Vector3.zero;


        if(useGameObject){
            createGridObjects();
        }else if(!useGameObject){
            createGridAbstract();
        }

        this.transform.eulerAngles = originalRotation;
    }


    //creates a grid of defined gridUnits
    private void createGridObjects(){
        Vector3 position = this.transform.position;
        float distanceBetweenUnits;


        gridUnit.transform.localScale = new Vector3(scaleX, scaleY, scaleZ);//sets the size of a grid unit
        gridObjects = new GameObject[columns, rows, height];//creates an array of gameobjects, 
        numberOfGridUnits = gridObjects.Length;//store number of grid units

        //creates the height of a grid
        for (int i = 0; i < height; i++){
            //creates a grid, start from the top left
            for(int j=0;j<rows;j++){//for each column
                for(int k=0;k<columns;k++){//for each row
                    gridObjects[k, j, i] = Instantiate(gridUnit, position, gameObject.transform.rotation,this.transform);//create a new gridUnit object at the position
                    
                    //used to verify positions
                    Debug.Log("Index: [" +
                    k + "," + j + "," + i + "]" +
                    "Value: " + position);
                

                    distanceBetweenUnits = gridUnit.transform.localScale.x;//uses x scale of a gameobject to determine spacing
                    position.x += distanceBetweenUnits;//update position to space out the row
                }

                position.x = this.transform.position.x;//resets x position for recreating the row
                distanceBetweenUnits = gridUnit.transform.localScale.y;//uses z scale of a gameobject to determine spacing
                position.y += distanceBetweenUnits;//update position to space out the column
            }

            position.x = this.transform.position.x;//resets x position (start from left again)
            position.y = this.transform.position.y;//resets y position (start from top again)
            distanceBetweenUnits = gridUnit.transform.localScale.z;//uses y scale of a gameobject to determine spacing between the 'layers'
            position.z += distanceBetweenUnits;//update position to space out the height (adding a 'layer')
        }
    }

    //creates a more abstract grid using an array of vector3
    private void createGridAbstract(){
        Vector3 position = this.transform.position;
        float distanceBetweenUnits;


        gridUnit.transform.localScale = new Vector3(scaleX, scaleY, scaleZ);//sets the size of a grid unit
        grid = new Vector3[columns, height, rows];//creates an array of vector3 to store positions of the 'center' of a grid unit
        numberOfGridUnits = grid.Length;//store number of grid units

        //creates the height of a grid
        for (int i = 0; i < height; i++)
        {
            //creates a grid, start from the top left
            for (int j = 0; j < columns; j++)
            {//for each column
                for (int k = 0; k < rows; k++)
                {//for each row
                    grid[j, i, k] = position;//stores stores a grid unit position

                    distanceBetweenUnits = gridUnit.transform.localScale.x;//uses x scale of a gameobject to determine spacing
                    position.x += distanceBetweenUnits;//update position to space out the row
                }

                position.x = this.transform.position.x;//resets x position for recreating the row
                distanceBetweenUnits = gridUnit.transform.localScale.y;//uses z scale of a gameobject to determine spacing
                position.y += distanceBetweenUnits;//update position to space out the column
            }

            position.x = this.transform.position.x;//resets x position (start from left again)
            position.y = this.transform.position.y;//resets y position (start from top again)
            distanceBetweenUnits = gridUnit.transform.localScale.z;//uses y scale of a gameobject to determine spacing between the 'layers'
            position.z += distanceBetweenUnits;//update position to space out the height (adding a 'layer')
        }

        if(visualizeGrid){
            attach2DGridPlane();
        }
    }

    //used to help visualize the abstract grid
    private void attach2DGridPlane(){
        planeGrid=Instantiate(gridUnit, this.transform.position,this.transform.rotation,this.transform);
        planeGrid.transform.localScale = new Vector3(columns, rows, rows);

    }

    //build a navmesh at runtime
    private void buildNavmesh(){
        if(gridObjects!=null)
        for (int i = 0; i <height; i++){
            for(int j=0;j<rows;j++){
                for(int k=0;k<columns;k++){
                    gridObjects[k,j,i].GetComponent<NavMeshSurface>().BuildNavMesh();
                }
            }
        }
    }
}
