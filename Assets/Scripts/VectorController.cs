using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// Instantiate a rigidbody then set the velocity
using System;

public class VectorController : MonoBehaviour
{
    //CameraManagement
    public Camera[] cameras;
    private int currentCameraIndex;
    public GameObject toTrack;
    public Vector3 offset;

    //Two vector arrows
    public GameObject prefab;
    private GameObject bodyArrow;
    private GameObject wingArrow;

    //Holds current vector position
    public Vector4[] vectorArray;
    
    //Text for current Matrix
    public Text cM1;
    public Text cM2;
    public Text cM3;
    public Text cM4;
    //Multiplicative Matrix
    public GameObject rotationMatrix;
    public Text rM1;
    public Text rM2;
    public Text rM3;
    public Text rM4;
    //For file iteration
    private int lineCounter;
    private String[] text;

    //tracks changes for use in rotation change of base
    private Double xChange;
    private Double yChange;
    private Double zChange;

    //Arrow has a height of 7 units
    void Start()
    {
        currentCameraIndex = 0;
        
        //Turn all cameras off, except the first default one
        for (int i=1; i<cameras.Length; i++) 
        {
            cameras[i].gameObject.SetActive(false);
        }
        
        //If any cameras were added to the controller, enable the first one
        if (cameras.Length>0)
        {
            cameras [0].gameObject.SetActive (true);
            Debug.Log ("Camera with name: " + cameras [0].GetComponent<Camera>().name + ", is now enabled");
        }

        xChange = yChange = zChange = 0;
        text = System.IO.File.ReadAllLines("Assets/Scripts/input.txt");
        lineCounter = 0;
        vectorArray = new Vector4[] {new Vector4(7, 0, 0, 1), new Vector4(0, 0, 0, 1), new Vector4(3.5F, 0, 3.5F, 1), new Vector4(3.5F, 0, -3.5F, 1)};
        cM1.text = String.Format("{0,-10: 0.00;-0.00; 0.00}{1,-10: 0.00;-0.00; 0.00}{2,-10: 0.00;-0.00; 0.00} 1", this.vectorArray[0].x, this.vectorArray[0].y, this.vectorArray[0].z);
        cM2.text = String.Format("{0,-10: 0.00;-0.00; 0.00}{1,-10: 0.00;-0.00; 0.00}{2,-10: 0.00;-0.00; 0.00} 1", this.vectorArray[1].x, this.vectorArray[1].y, this.vectorArray[1].z);
        cM3.text = String.Format("{0,-10: 0.00;-0.00; 0.00}{1,-10: 0.00;-0.00; 0.00}{2,-10: 0.00;-0.00; 0.00} 1", this.vectorArray[2].x, this.vectorArray[2].y, this.vectorArray[2].z);
        cM4.text = String.Format("{0,-10: 0.00;-0.00; 0.00}{1,-10: 0.00;-0.00; 0.00}{2,-10: 0.00;-0.00; 0.00} 1", this.vectorArray[3].x, this.vectorArray[3].y, this.vectorArray[3].z);
        bodyArrow = Instantiate(prefab, vectorArray[1], Quaternion.identity);
        fixArrow(vectorArray[0], vectorArray[1], bodyArrow);
        wingArrow = Instantiate(prefab, vectorArray[3], Quaternion.identity);
        fixArrow(vectorArray[2], vectorArray[3], wingArrow);

        toTrack = bodyArrow;
    }

    void LateUpdate() {
        cameras[3].gameObject.transform.position = toTrack.transform.position + offset;
    }

    void Update()
    {
        if(Input.GetKey("space")) {
            cM1.text = String.Format("{0,-9: 0.00;-0.00; 0.00}{1,-9: 0.00;-0.00; 0.00}{2,-9: 0.00;-0.00; 0.00} 1", this.vectorArray[0].x, this.vectorArray[0].y, this.vectorArray[0].z);
            cM2.text = String.Format("{0,-9: 0.00;-0.00; 0.00}{1,-9: 0.00;-0.00; 0.00}{2,-9: 0.00;-0.00; 0.00} 1", this.vectorArray[1].x, this.vectorArray[1].y, this.vectorArray[1].z);
            cM3.text = String.Format("{0,-9: 0.00;-0.00; 0.00}{1,-9: 0.00;-0.00; 0.00}{2,-9: 0.00;-0.00; 0.00} 1", this.vectorArray[2].x, this.vectorArray[2].y, this.vectorArray[2].z);
            cM4.text = String.Format("{0,-9: 0.00;-0.00; 0.00}{1,-9: 0.00;-0.00; 0.00}{2,-9: 0.00;-0.00; 0.00} 1", this.vectorArray[3].x, this.vectorArray[3].y, this.vectorArray[3].z);String[] tempText = text[lineCounter].Split(' ');
            if (tempText[0].Equals("m")) {
                xChange += Double.Parse(tempText[1]);
                yChange += Double.Parse(tempText[2]);
                zChange += Double.Parse(tempText[3]);
                Vector4[] transform = new Vector4[] {new Vector4(1, 0, 0, 0), new Vector4(0, 1, 0, 0), new Vector4(0, 0, 1, 0), new Vector4((float)Double.Parse(tempText[1]), (float)Double.Parse(tempText[2]), (float)Double.Parse(tempText[3]), 1)};
                updateDisplay(transform);
                this.vectorArray = Multiply(transform, this.vectorArray);
            }
            else if (tempText[0].Equals("r")) {
                double x = Double.Parse(tempText[1]);
                double y = Double.Parse(tempText[2]);
                double z = Double.Parse(tempText[3]);
                Vector4[] rotate = Rotate(x * Math.PI / 180, y * Math.PI / 180, z * Math.PI / 180);
                updateDisplay(rotate);
                this.vectorArray = Multiply(rotate, this.vectorArray);
            }
            else if (tempText[0].Equals("c")) {
                currentCameraIndex ++;
                if (currentCameraIndex < cameras.Length)
                {
                    cameras[currentCameraIndex-1].gameObject.SetActive(false);
                    cameras[currentCameraIndex].gameObject.SetActive(true);
                    Debug.Log ("Camera with name: " + cameras [currentCameraIndex].GetComponent<Camera>().name + ", is now enabled");
                }
                else
                {
                    cameras[currentCameraIndex-1].gameObject.SetActive(false);
                    currentCameraIndex = 0;
                    cameras[currentCameraIndex].gameObject.SetActive(true);
                    Debug.Log ("Camera with name: " + cameras [currentCameraIndex].GetComponent<Camera>().name + ", is now enabled");
                }
            }
            lineCounter++;

        }

        fixArrow(vectorArray[0], vectorArray[1], bodyArrow);
        fixArrow(vectorArray[2], vectorArray[3], wingArrow);
        
    }

    void fixArrow(Vector3 arrowHead, Vector3 arrowTail, GameObject arrow)
    {
        arrow.transform.position = arrowTail;
        arrow.transform.LookAt(arrowHead);
        Vector3 tmp = arrow.transform.rotation.eulerAngles;
        tmp.x += 90;
        arrow.transform.rotation = Quaternion.Euler(tmp);
    }
    
    //Input is a 4x4 matrix
    //Multiplies subject by newMatrix and returns the resulting matrix
    Vector4[] Multiply(Vector4[] newMatrix, Vector4[] subject) {
        Vector4 [] newArray = new Vector4[4]; 
        for(int j = 0; j < 4; j++) {
            newArray[j] = new Vector4(subject[j][0]*newMatrix[0][0] + subject[j][1]*newMatrix[1][0] + subject[j][2]*newMatrix[2][0] + subject[j][3]*newMatrix[3][0],
                                        subject[j][0]*newMatrix[0][1] + subject[j][1]*newMatrix[1][1] + subject[j][2]*newMatrix[2][1] + subject[j][3]*newMatrix[3][1],
                                        subject[j][0]*newMatrix[0][2] + subject[j][1]*newMatrix[1][2] + subject[j][2]*newMatrix[2][2] + subject[j][3]*newMatrix[3][2],
                                        subject[j][0]*newMatrix[0][3] + subject[j][1]*newMatrix[1][3] + subject[j][2]*newMatrix[2][3] + subject[j][3]*newMatrix[3][3]);
        }
        return newArray;

    }
    //All angles in radians
    /*Reverse with command
    r -angleX 0 0
    r 0 -angleY 0
    r 0 0 -angleZ
    */
    Vector4[] Rotate(double angleX, double angleY, double angleZ) 
    {
        Vector4[] multMatrix = new Vector4[4];
        multMatrix[0] = new Vector4((float)(Math.Cos(angleZ)*Math.Cos(angleY)), (float)(Math.Cos(angleZ)*Math.Sin(angleY)*Math.Sin(angleX) - Math.Sin(angleZ)*Math.Cos(angleX)), (float)(Math.Cos(angleZ)*Math.Sin(angleY)*Math.Cos(angleX) + Math.Sin(angleZ)*Math.Sin(angleX)), 0);
        multMatrix[1] = new Vector4((float)(Math.Sin(angleZ)*Math.Cos(angleY)), (float)(Math.Sin(angleZ)*Math.Sin(angleY)*Math.Sin(angleX) + Math.Cos(angleZ)*Math.Cos(angleX)), (float)(Math.Sin(angleZ)*Math.Sin(angleY)*Math.Cos(angleX) - Math.Cos(angleZ)*Math.Sin(angleX)), 0);
        multMatrix[2] = new Vector4((float)(-1 * Math.Sin(angleY)), (float)(Math.Cos(angleY)*Math.Sin(angleX)), (float)(Math.Cos(angleY)*Math.Cos(angleX)), 0);
        multMatrix[3] = new Vector4(0, 0, 0, 1);

        Vector4[] toOrigin = new Vector4[] {new Vector4(1, 0, 0, 0), new Vector4(0, 1, 0, 0), new Vector4(0, 0, 1, 0), new Vector4((float)(-1 * xChange), (float)(-1 * yChange), (float)(-1 * zChange), 1)};
        Vector4[] fromOrigin = new Vector4[] {new Vector4(1, 0, 0, 0), new Vector4(0, 1, 0, 0), new Vector4(0, 0, 1, 0), new Vector4((float)xChange, (float)yChange, (float)zChange, 1)};

        return Multiply(fromOrigin, Multiply(multMatrix, toOrigin));
    }

    void updateDisplay(Vector4[] toDisplay) {
        rM1.text = String.Format("{0,-5:0.0} {1,-5:0.0} {2,-5:0.0} 1", toDisplay[0].x, toDisplay[0].y, toDisplay[0].z);
        rM2.text = String.Format("{0,-5:0.0} {1,-5:0.0} {2,-5:0.0} 1", toDisplay[1].x, toDisplay[1].y, toDisplay[1].z);
        rM3.text = String.Format("{0,-5:0.0} {1,-5:0.0} {2,-5:0.0} 1", toDisplay[2].x, toDisplay[2].y, toDisplay[2].z);
        rM4.text = String.Format("{0,-5:0.0} {1,-5:0.0} {2,-5:0.0} 1", toDisplay[3].x, toDisplay[3].y, toDisplay[3].z);
        rotationMatrix.SetActive(true);
    }

}

//XRotate
//Multiply(new Vector4[] {new Vector4(1, 0, 0, 0), new Vector4(0, (float)Math.Cos(angle), (float)(-1*Math.Sin(angle)), 0), new Vector4(0, (float)Math.Sin(angle), (float)Math.Cos(angle), 0), new Vector4(0, 0, 0, 1)});

//YRotate
//Multiply(new Vector4[] {new Vector4((float)Math.Cos(angle), 0, (float)Math.Sin(angle), 0), new Vector4(0, 1, 0, 0), new Vector4((float)(-1*Math.Sin(angle)), 0, (float)Math.Cos(angle), 0), new Vector4(0, 0, 0, 1)});

//ZRotate
//this.vectorArray = Multiply(new Vector4[] {new Vector4((float)Math.Cos(angle), (float)(-1*Math.Sin(angle)), 0, 0), new Vector4((float)Math.Sin(angle), (float)Math.Cos(angle), 0, 0), new Vector4(0, 0, 1, 0), new Vector4(0, 0, 0, 1)});
