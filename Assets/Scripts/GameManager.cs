using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour {
	
	public static GameManager instance
	{
		get { return inst_; }
		set {}
	}
	private static GameManager inst_;
	public static void Init()
	{
		if (inst_ == null)
		{
			GameObject mgr = Instantiate(Resources.Load("GameManager")) as GameObject;
			inst_ = mgr.GetComponent<GameManager>();
			DontDestroyOnLoad(mgr);
		}
	}

	public GUIText guiScore;
	
	private int score;
	
	
	
	
	public void AwardPoints(int points)
	{
		score += points;
 		guiScore.text = score.ToString();
	}
	
	
	
	
	// Use this for initialization
	void Start () 
	{
		
	}
	
	// Update is called once per frame
	void Update ()
	{
	
	}
}
