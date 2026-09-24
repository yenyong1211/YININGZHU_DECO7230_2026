using UnityEngine;

public class WordCardTrigger : MonoBehaviour
{
    public ObjectWordCard wordCardManager;
    public Task1Manager task1Manager;

    public void ShowCard()
    {
        wordCardManager.ShowWordCard(gameObject);

        if (task1Manager != null)
        {
            task1Manager.CheckObject(gameObject);
        }
    }
}