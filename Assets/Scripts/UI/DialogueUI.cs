using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.Dialogue;
using TMPro;
using UnityEngine.UI;

namespace RPG.UI
{
    public class DialogueUI : MonoBehaviour
    {
        [SerializeField] TextMeshProUGUI AIText;
        PlayerConversant playerConversant;
        [SerializeField] Button nextButton;
        [SerializeField] Button quitButton;
        [SerializeField] GameObject AIResponse;
        [SerializeField] Transform choiceRoot;
        [SerializeField] GameObject choicePrefab;
        [SerializeField] TextMeshProUGUI conversantName;
       

        // Start is called before the first frame update
        void Start()
        {
            playerConversant = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerConversant>();

            //subscribing to events
            playerConversant.onConversationUpdated += UpdateUI;
            nextButton.onClick.AddListener(Next); //manually subscribing to the click on the Next Button
            quitButton.onClick.AddListener(() => { playerConversant.Quit(); }); //created a new function on the fly, taking place of an imaginary function in this script.

            UpdateUI();
        }        

        private void Next()
        {
            playerConversant.Next();            
        }

        private void UpdateUI()
        {
            gameObject.SetActive(playerConversant.IsActive());
            if (!playerConversant.IsActive()) 
            { 
                return; 
            }

            conversantName.text = playerConversant.GetCurrentConversantName();
            AIResponse.SetActive(!playerConversant.IsChoosing());
            choiceRoot.gameObject.SetActive(playerConversant.IsChoosing());

            if (playerConversant.IsChoosing())
            {
                BuildChoiceList();
            }
            else
            {
                AIText.text = playerConversant.GetText();
                nextButton.gameObject.SetActive(playerConversant.hasNext());
            }
        }

        private void BuildChoiceList()
        {
            foreach (Transform childTransform in choiceRoot)
            {
                Destroy(childTransform.gameObject);
            }

            foreach (DialogueNode choice in playerConversant.GetChoices())
            {
                GameObject choiceInstance = Instantiate(choicePrefab, choiceRoot);
                TextMeshProUGUI textComp = choiceInstance.GetComponentInChildren<TextMeshProUGUI>();
                textComp.text = choice.GetNodeText();
                Button button = choiceInstance.GetComponentInChildren<Button>();
                button.onClick.AddListener(() => 
                {
                    playerConversant.SelectChoice(choice); //lambda function - created during the foreach loop, for each button seperately (context dependent).                   
                });
            }
        }
    }
}
