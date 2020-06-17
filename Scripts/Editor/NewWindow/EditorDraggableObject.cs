using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

//A draggable object inside an editor windows
public class EditorDraggableObject
{
	private bool isDragged;
	private Vector2 position;
	private Vector2 offsetPosition;

	public EditorDraggableObject()
	{
		position = new Vector2(20, 20);
	}

    public void OnGUI()
    {
		Rect drawRect = new Rect(position.x, position.y, 100, 100);

		GUIStyle style = new GUIStyle();
		GUIStyleState gUIStyleState = new GUIStyleState();
		Texture2D bg = new Texture2D(1, 1);
		bg.SetPixel(1, 1, Color.red);
		bg.wrapMode = TextureWrapMode.Repeat;
		bg.Apply();
		gUIStyleState.background = bg;
		style.normal = gUIStyleState;

		GUILayout.BeginArea(drawRect);
		GUILayout.Label("Test\nBelow test", style, GUILayout.ExpandWidth(true));
		GUILayout.EndArea();

		if(Event.current.type == EventType.MouseUp)
		{
			isDragged = false;

		} else if (Event.current.type == EventType.MouseDown && drawRect.Contains(Event.current.mousePosition)) //Check if mouse is clicked inside box
		{
			isDragged = true;
			offsetPosition = Event.current.mousePosition - drawRect.position;
			Debug.Log("You clicked me");
			Event.current.Use();
		}

		if (isDragged)
		{
			position = Event.current.mousePosition - offsetPosition;
		}
	}

	private void OnMouseEnter()
    {
        Debug.Log("Mouse entered me");
    }
}
