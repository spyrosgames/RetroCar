using UnityEngine;
using System.Collections;

public class BaseUI {
	
	
	protected float scrollViewSize;
	protected string viewTopicTitle;
	
	protected static float modelScreenWidth = 320.0f;
	protected static float modelScreenHeight = 480.0f;	
	
	protected static float titleBarHeight = 20.0f;
	protected static float titleBarEdgeMargin = 10.0f;
	
	protected static float menuButtonHeight = 40.0f;
	protected static float menuWindowWidth = 110.0f;
	protected static float menuTopMargin = 25.0f;
	protected static float menuVerticalInterButtonMargin = 15.0f;		
	protected static float menuLeftMargin = 5.0f;
	protected static float menuLeftInsetFromScreen = 0.0f;
	protected static float menuRightPadContentMargin = 5.0f;
	protected static float menuLeftWidth = menuLeftInsetFromScreen + menuWindowWidth + (menuRightPadContentMargin * 2);
		
	// Measure variables
	protected static float verticalScrollBarHorizontalMargin = 17.0f;
	protected static float bottomScrollViewScreenMargin = 5.0f;
	protected static float backButtonSquareDimension = 50.0f;
	protected static float boxHeight = 160.0f;
	protected static float boxWidth = 170.0f;		
	protected static float buttonHeight = 60.0f;
	protected static float buttonWidth = 160.0f;
	protected static float textEditHeight = 40.0f;
	protected static float textEditWidth = 160.0f;
	protected static float boxTitleSeparator = 30.0f;
	protected static float heightSeparator = 10.0f;
	protected static float groupHeightSeparator = heightSeparator * 2;
	protected static float scrollViewHorizontalInsetMargin = 5.0f;
	protected static float scrollViewTopInsetMargin = 20.0f;
	
	protected static float scaleWidth = Screen.width / modelScreenWidth;  
	protected static float scaleHeight = Screen.height / modelScreenHeight;  
    //create a scale Vector3 with the above ratio  
    protected static Vector3 screenScale = new Vector3(scaleWidth, scaleHeight, 1.0f); 
	protected static Vector3 positionVector = new Vector3(screenScale.x, screenScale.y, 0.0f);
	
	
	protected static float testWindowViewX = ((modelScreenWidth - menuLeftWidth) / 2 - (boxWidth + verticalScrollBarHorizontalMargin + (scrollViewHorizontalInsetMargin * 2.0f)) / 2) + menuLeftWidth;
	protected static float testWindowViewY = groupHeightSeparator + backButtonSquareDimension + titleBarHeight;
	protected static float testWindowViewHeight = modelScreenHeight - titleBarHeight - groupHeightSeparator - backButtonSquareDimension - scrollViewTopInsetMargin;

	protected static Rect testWindowViewRect = new Rect (testWindowViewX, testWindowViewY, boxWidth + verticalScrollBarHorizontalMargin + (scrollViewHorizontalInsetMargin * 2.0f), testWindowViewHeight);
	
	
    
	
	
	/// <summary>
	/// Executor class with input text field.
	/// </summary>
	protected interface IInputExecutor {
		
		void execute(object[] input);
		
	}
	
	// <summary>
	// Executor class with no arguments.
	// </summary>
	protected interface IExecutor {
		
		void execute();
		
	}
	
	// <summary>
	// Executor class with no arguments.
	// </summary>
	protected interface ITopic {
		string menuItem();
		int serviceCount();
		void drawTopic();
	}
	
	
	public Matrix4x4 scalingMatrix {
		get {
			return Matrix4x4.TRS(positionVector, Quaternion.identity, screenScale);
		}
	}
	
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	protected void DrawTitleBar(string leftText, string middleText, string rightText) {
		TextAnchor savedAlignment = GUI.skin.label.alignment;
		
		int labelsUsedCount = 0;
		if (leftText != null) labelsUsedCount += 1;
		if (middleText != null) labelsUsedCount += 1;
		if (rightText != null) labelsUsedCount += 1;
		float labelWidth = (modelScreenWidth - titleBarEdgeMargin * 2.0f) / (float)labelsUsedCount;
		
		GUI.Box(new Rect (0.0f, 0.0f, modelScreenWidth, titleBarHeight), "");
		if (leftText != null) {
			GUI.skin.label.alignment = TextAnchor.MiddleLeft;
			GUI.Label(new Rect(titleBarEdgeMargin, 0.0f, labelWidth, titleBarHeight), leftText);
		}
		if (middleText != null) {
			GUI.skin.label.alignment = TextAnchor.MiddleCenter;
			GUI.Label(new Rect((modelScreenWidth / 2.0f - labelWidth / 2.0f), 0.0f, labelWidth, titleBarHeight), middleText);
		}
		if (rightText != null) {
			GUI.skin.label.alignment = TextAnchor.MiddleRight;
			GUI.Label(new Rect(modelScreenWidth - labelWidth - titleBarEdgeMargin, 0.0f, labelWidth, titleBarHeight), rightText);
		}

		GUI.skin.label.alignment = savedAlignment;
	}
	
	protected static Rect menuButtonRectForButton(int buttonIndex) {
		return new Rect (menuLeftMargin, menuButtonHeight * buttonIndex + menuTopMargin + menuVerticalInterButtonMargin * buttonIndex, menuWindowWidth - menuLeftMargin * 2, menuButtonHeight);
	}
	
	protected Vector2 scrollView(Vector2 scrollPos, int servicesCount, float scrollViewSize) {
		scrollPos = GUI.BeginScrollView (
			new Rect (scrollViewHorizontalInsetMargin, scrollViewTopInsetMargin, boxWidth + verticalScrollBarHorizontalMargin, testWindowViewHeight - scrollViewTopInsetMargin - bottomScrollViewScreenMargin), 
			scrollPos, 
			new Rect (0.0f, 0.0f, boxWidth, scrollViewSize)
		);
		return scrollPos;
	}
	//			new Rect (scrollViewX, groupHeightSeparator, boxWidth, (boxHeight + groupHeightSeparator) * servicesCount - backButtonSquareDimension)
	
	
	/// <summary>
	/// It draws a group with an input field, label and a button. When button is clicked, given executor will get executed.
	/// </summary>
	/// <param name="boxTitle">
	/// The title of the box.
	/// </param>
	/// <param name="buttonText">
	/// Label for the button.
	/// </param>
	/// <param name="groupStartY">
	/// Start position for Y coordinate.
	/// </param>
	/// <param name="executor">
	/// To be executed when button is clicked.
	/// </param>
	protected float drawGroupWithInput(string boxTitle, string buttonText, float groupStartY, IInputExecutor executor, object[,] fieldsAndValues) {
		
		Vector2 startPoint = new Vector2(0, 0);
		float boxHeight = (heightSeparator*3+textEditHeight)*fieldsAndValues.GetLength(0)+buttonHeight+heightSeparator*4;
		GUI.BeginGroup(new Rect (0.0f, groupStartY, boxWidth, boxHeight));
		GUI.Box(new Rect (0, 0, boxWidth, boxHeight), boxTitle);
		
		startPoint.y += boxTitleSeparator;
		
		object[] values = new object[fieldsAndValues.Length];
		for (int x = 0; x < fieldsAndValues.GetLength(0); x++) 
		{
		 GUI.Label(new Rect (boxWidth / 2 - textEditWidth / 2, startPoint.y, textEditWidth, heightSeparator*2), (string) fieldsAndValues[x,0]);
		 startPoint.y += heightSeparator*2;
		 if (fieldsAndValues.GetLength(1) == 2 || isTextField((string) fieldsAndValues[x,2]))	
		 	values[x] = GUI.TextField(new Rect (boxWidth / 2 - textEditWidth / 2, startPoint.y, textEditWidth, textEditHeight), (string) fieldsAndValues[x,1]);
		 else if (isToggle((string) fieldsAndValues[x,2]))
			values[x] = GUI.Toggle(new Rect (boxWidth / 2 - textEditWidth / 2, startPoint.y, textEditWidth, textEditHeight), (bool) fieldsAndValues[x,1], "textttt");
			
		 fieldsAndValues[x,1] = values[x];
		 startPoint.y += textEditHeight + heightSeparator;
		}
		
		if (GUI.Button(new Rect (boxWidth / 2 - buttonWidth / 2, startPoint.y, buttonWidth, buttonHeight), buttonText)) {
			executor.execute(values);
		}

		GUI.EndGroup();
		
		return boxHeight + groupHeightSeparator;
	}
	
	
	private bool isTextField(string value) {
		return "textfield".Equals(value);
	}

	private bool isToggle(string value) {
		return "toggle".Equals(value);
	}

	/// <summary>
	/// It draws a group with label and a button. When button is clicked, given executor will get executed.
	/// </summary>
	/// <param name="boxTitle">
	/// The title of the box.
	/// </param>
	/// <param name="buttonText">
	/// Label for the button.
	/// </param>
	/// <param name="groupStartY">
	/// Start position for Y coordinate.
	/// </param>
	/// <param name="executor">
	/// To be executed when button is clicked.
	/// </param>
	protected float drawGroup(string boxTitle, string buttonText, float groupStartY, IExecutor executor) {
		
		Vector2 startPoint = new Vector2(0, 0);
		GUI.BeginGroup(new Rect (0.0f, groupStartY, boxWidth, boxHeight));
		GUI.Box(new Rect (0, 0, boxWidth, boxHeight), boxTitle);
		
		startPoint.y += boxTitleSeparator * 2;		
		if (GUI.Button(new Rect (boxWidth / 2 - buttonWidth / 2, startPoint.y, buttonWidth, buttonHeight), buttonText)) {
			executor.execute();
		}

		GUI.EndGroup();
		
		return boxHeight + groupHeightSeparator;
	}
	
}
