using UnityEngine;
using UnityEngine.UI;
using TMPro;
using LUT.Events.Primitives;

[RequireComponent(typeof(Animation))]
public class FeedbackScreen : MonoBehaviour
{
	[SerializeField]
	public EventBool onGameEnd;

	public TextMeshProUGUI feedbackText;

	public AnimationClip inAnimation;
	public AnimationClip outAnimation;

	private new Animation animation;
	private GameFlowController cachedGameFlow;

	protected void Start()
	{
		animation = GetComponent<Animation>();
		onGameEnd.Register(OnGameEnd);
		cachedGameFlow = GameFlowController.Instance;
	}

	private void OnDestroy()
	{
		onGameEnd.Unregister(OnGameEnd);
	}

	public void OnGameEnd(bool state)
	{
		feedbackText.text = state ? "You win" : "You Lose";
		animation.Play(inAnimation.name);
	}


	public void FinishScene()
	{
		cachedGameFlow.ContinueGame();
		animation.Play(outAnimation.name);
	}
}
