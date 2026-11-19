using System;
using TMPro;
using UnityEngine;

public class ChangePseudo : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _pseudo;
    [SerializeField] private TextMeshProUGUI _pseudoToChange;
    private void OnEnable()
    {
        refreshpsuedoPlayer();
    }

    public void changePseudo()
    {
        PlayerIdentity.SetPseudo(_pseudo.text);
        refreshpsuedoPlayer();
    }

    public void refreshpsuedoPlayer()
    {
        string pseudo ;
        pseudo = PlayerIdentity.GetPseudo();
        Debug.Log(pseudo);
        _pseudoToChange.text = pseudo;
    }
}
