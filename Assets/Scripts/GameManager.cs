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
	
	void Awake()
	{
		score = 0;
		playerState = new ActorState(null);  // transform.Find ("Healthbar").GetComponent<Renderer>();
	}
	
	
	public GUIText guiScore;
	
	private int score;
	
	public ActorState playerState;
	public Rect healthBarRect;
	
	public Texture healthbarTexture;
	
	void OnGUI() {
        if (Event.current.type.Equals(EventType.Repaint))
            Graphics.DrawTexture(healthBarRect, healthbarTexture, new Rect(playerState.getHealthRatioMissing() * 0.5f, 0, 0.5f, 1.0f), 0, 0, 0, 0);
        
    }
	
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
