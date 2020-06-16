using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class lifeBar : MonoBehaviour
{
    [SerializeField] Image lifeBarGUI;
    [SerializeField] float actualLife;
    [SerializeField] float maxLife;
    [SerializeField] float speed = 3;
    private float oldLife;
    private float currentTime = 0; 
    private float normalizedValue;
    private Vector2 oldLifeBarSize;
    private Vector2 lifeBarSize = new Vector2();
    private Animation lifeBarAnim;


    void Start()
    {
        actualLife = maxLife;
        oldLife = actualLife;
        oldLifeBarSize = lifeBarSize;
        lifeBarAnim = lifeBarGUI.GetComponent<Animation>();
        lifeBarSize = lifeBarGUI.rectTransform.sizeDelta;
    }

    void FixedUpdate()
    {
        if (actualLife >= maxLife) //impede que a vida ultrapasse o máximo
        {
            actualLife = maxLife;
        }
        else if (actualLife != oldLife) //quando a vida atual mudar inicia a corrotina
        {
            IEnumerator cachedCoroutine = LerpLifeBar();
            StartCoroutine(cachedCoroutine);
        }

        if (actualLife / maxLife * 100 <= 10)
        {
            lifeBarAnim.Play("lowLife");
        }
        else
        {
            lifeBarAnim.Stop("lowLife");
            lifeBarGUI.color = new Color(0.85849F, 0.044544F, 0.044544F, 0.89412F);
        }
    }

    //Cria uma corrotina que vai monitorar e dar update visual na life bar
    IEnumerator LerpLifeBar()
    {

        while (currentTime <= speed) //essa parte ainda ta com problema, em execução faz exatamente o que quero, alterando a vida com base em uma velocidade definida no inspecto, entretanto o while nunca vai finalizar
            // se eu tentar organizar o código para ele finalizar, o movimento learp não funciona, como ta agr pode gerar problemas de desempenho e precisa ser melhorado
        {
            if (currentTime < Time.fixedDeltaTime) 
            {
                currentTime += Time.fixedDeltaTime;
                normalizedValue = currentTime / speed;
                lifeBarGUI.rectTransform.sizeDelta = Vector2.Lerp(oldLifeBarSize, new Vector2(actualLife / maxLife * lifeBarSize.x, lifeBarSize.y), normalizedValue);
            }
            else
            {
                oldLifeBarSize = lifeBarGUI.rectTransform.sizeDelta;
                currentTime = 0;
                oldLife = actualLife;
                yield return null;
            }
            //Debug.Log(currentTime);
        }
    }
}
