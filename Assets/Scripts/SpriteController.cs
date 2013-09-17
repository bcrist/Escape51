using UnityEngine;

using System.Collections.Generic;
using System.Xml;

class SpriteImage {
	public Vector2 offset;
	public Vector2 scale;
}

class SpriteFrame {
	public SpriteImage image;
	public double duration;
}

class SpriteAnimation {
	
	public SpriteAnimation()
	{
		frames = new List<SpriteFrame>();
		nextAnimationId = -1;
	}
	
	public List<SpriteFrame> frames;
	public int nextAnimationId;
}

public class SpriteController : MonoBehaviour {
	
	public TextAsset spriteData;
	
	public int initialAnimationId;
	
	private List<SpriteImage> images;
	private List<SpriteAnimation> animations;
	
	private Queue<SpriteAnimation> animationQueue;

	private int frame;	     // The number of frames that have been displayed from this animation.
	private double deltaT;	 // time since last frame change
	private double thresholdT; // When deltaT >= thresholdT, show next frame.
	
	void Start()
	{
		images = new List<SpriteImage>();
		animations = new List<SpriteAnimation>();
		animationQueue = new Queue<SpriteAnimation>();
		
		ParseXmlSpriteData();
		
		frame = 0;
		deltaT = 0;
		thresholdT = 1.0;
		
		startAnimation (initialAnimationId);
	}
	
	// Start an animation immediately -- cancels any running or queued animations.
	public void startAnimation(int id)
	{
		animationQueue.Clear();
		
		if (id >= 0 && id < animations.Count)
		{
			animationQueue.Enqueue (animations[id]);
		}
		
		frame = 0;
		deltaT = 0;
		NextFrame();
	}
	
	public void queueAnimation(int id)
	{
		if (id >= 0 && id < animations.Count)
		{
			animationQueue.Enqueue (animations[id]);
			
			if (animationQueue.Count == 1)
			{
				frame = 0;
				deltaT = 0;
				NextFrame();
			}
		}
	}
	
	void Update()
	{
		deltaT += Time.deltaTime;
		
		if (deltaT > thresholdT)
		{
			deltaT -= thresholdT;
			NextFrame();
		}
	}
	
	void NextFrame()
	{
		while (animationQueue.Count > 0)
		{
			SpriteAnimation anim = animationQueue.Peek();
			
			if (frame >= 0 && frame < anim.frames.Count)
			{
				SpriteFrame f = anim.frames[frame];
				renderer.material.mainTextureOffset = f.image.offset;
				renderer.material.mainTextureScale = f.image.scale;
				thresholdT = f.duration;
				++frame;
				return;
			}
			
			// If this is the end of the current animation, see if there is another one queued by dequeuing the current and looping.
			frame = 0;
			SpriteAnimation lastAnimation = animationQueue.Dequeue();
			
			if (animationQueue.Count == 0 && lastAnimation.nextAnimationId >= 0 && lastAnimation.nextAnimationId < animations.Count)
			{
			 	// If we get here, there are no animations left on the queue, see if the last animation has a "next" animation
				animationQueue.Enqueue(animations[lastAnimation.nextAnimationId]);
			}
		}
	}
	
	void OnDestroy()
	{
        DestroyImmediate(renderer.material);
    }
	
	void ParseXmlSpriteData()
	{
		images.Clear();
		animations.Clear();
		
		// Load XML data & initialize images/animations
		XmlDocument doc = new XmlDocument();
		doc.LoadXml(spriteData.text);
		
		XmlElement root = doc.DocumentElement;
		
		if (root.Name != "spritedata")
		{
			Debug.Log("Invalid XML sprite definition file!");
			return;
		}
		
		float xscale;
		if (!float.TryParse(root.GetAttribute("xscale"), out xscale))
			xscale = 1.0f;
		
		float yscale;
		if (!float.TryParse(root.GetAttribute("yscale"), out yscale))
			yscale = 1.0f;
			
		float tscale;
		if (!float.TryParse(root.GetAttribute("timescale"), out tscale))
			tscale = 1.0f;
		
		foreach (XmlNode node in root.SelectNodes("./image"))
		{
			int id;
			float x;
			float y;
			float w;
			float h;
			XmlElement n;
			
			n = node["id"];
			if (n == null || !int.TryParse(n.InnerText, out id))
			{
				Debug.Log("Failed to parse sprite image ID: " + node["id"].InnerText);
				continue;
			}
			
			n = node["x"];
			if (n == null || !float.TryParse(n.InnerText, out x))
			{
				Debug.Log ("Failed to parse sprite image X for sprite " + id);
				continue;
			}
			
			n = node["y"];
			if (n == null || !float.TryParse(n.InnerText, out y))
			{
				Debug.Log ("Failed to parse sprite image Y for sprite " + id);
				continue;
			}
			
			n = node["w"];
			if (n == null || !float.TryParse(n.InnerText, out w))
			{
				Debug.Log ("Failed to parse sprite image width for sprite " + id);
				continue;
			}
			
			n = node["h"];
			if (n == null || !float.TryParse(n.InnerText, out h))
			{
				Debug.Log ("Failed to parse sprite image height for sprite " + id);
				continue;
			}
			
			SpriteImage image = new SpriteImage();
			image.offset = new Vector2(x / xscale, y / yscale);
			image.scale = new Vector2(w / xscale, h / yscale);
			
			while (images.Count <= id)
				images.Add(null);
			
			images[id] = image;
		}
		
		foreach (XmlNode node in root.SelectNodes("./animation"))
		{
			int id;
			int nextId;
			XmlElement n;
			
			n = node["id"];
			if (n == null || !int.TryParse(n.InnerText, out id))
			{
				Debug.Log ("Failed to parse animation ID: " + node["id"].InnerText);
				continue;
			}
			
			n = node["next"];
			if (n == null || !int.TryParse(n.InnerText, out nextId))
				nextId = -1;
			
			while (animations.Count <= id)
				animations.Add(null);
			
			SpriteAnimation anim = new SpriteAnimation();
			animations[id] = anim;
			anim.nextAnimationId = nextId;
			
			foreach (XmlNode frameNode in node.SelectNodes("./frame"))
			{
				XmlElement frameElem = (XmlElement)frameNode;
				SpriteFrame frame = new SpriteFrame();
				int imageId;
				float duration;
				
				if (!int.TryParse(frameElem.GetAttribute("id"), out imageId))
				{
					Debug.Log ("Failed to parse frame image id " + frameElem.GetAttribute("id") + " in animation " + id);
					continue;
				}
				
				if (imageId >= images.Count)
				{
					Debug.Log ("Invalid image id " + imageId + " in animation " + id);
					continue;
				}
				frame.image =  images[imageId];
				
				if (!float.TryParse(frameElem.GetAttribute("length"), out duration))
				{
					Debug.Log ("Failed to parse frame length for image id " + imageId + " in animation " + id);
					continue;
				}
				frame.duration = duration / tscale;
				
				anim.frames.Add (frame);
			}
		}
	}
}
