using System;
using UnityEngine;

namespace Complete
{
    [Serializable]
    public class TankManager
    {
        // This class is to manage various settings on a tank.
        // It works with the GameManager class to control how the tanks behave
        // and whether or not players have control of their tank in the 
        // different phases of the game.

        public Color m_PlayerColor;                             // This is the color this tank will be tinted.
        public Transform m_SpawnPoint;                          // The position and direction the tank will have when it spawns.
        [HideInInspector] public int playerNumber;            // This specifies which player this the manager for.
        [HideInInspector] public string coloredPlayerText;    // A string that represents the player with their number colored to match their tank.
        [HideInInspector] public GameObject instance;         // A reference to the instance of the tank when it is created.
        [HideInInspector] public int wins;                    // The number of wins this player has so far.
        

        private TankMovement movement;                        // Reference to tank's movement script, used to disable and enable control.
        private TankShooting shooting;                        // Reference to tank's shooting script, used to disable and enable control.
        private GameObject canvasGameObject;                  // Used to disable the world space UI during the Starting and Ending phases of each round.


        public void Setup ()
        {
            // Get references to the components.
            movement = instance.GetComponent<TankMovement> ();
            shooting = instance.GetComponent<TankShooting> ();
            canvasGameObject = instance.GetComponentInChildren<Canvas> ().gameObject;

            // Set the player numbers to be consistent across the scripts.
            movement.playerNumber = playerNumber;
            shooting.playerNumber = playerNumber;

            // Create a string using the correct color that says 'PLAYER 1' etc based on the tank's color and the player's number.
            coloredPlayerText = "<color=#" + ColorUtility.ToHtmlStringRGB(m_PlayerColor) + ">PLAYER " + playerNumber + "</color>";

            // Get all of the renderers of the tank.
            MeshRenderer[] renderers = instance.GetComponentsInChildren<MeshRenderer> ();

            // Go through all the renderers...
            for (int i = 0; i < renderers.Length; i++)
            {
                // ... set their material color to the color specific to this tank.
                renderers[i].material.color = m_PlayerColor;
            }
        }


        // Used during the phases of the game where the player shouldn't be able to control their tank.
        public void DisableControl ()
        {
            movement.enabled = false;
            shooting.enabled = false;

            canvasGameObject.SetActive (false);
        }


        // Used during the phases of the game where the player should be able to control their tank.
        public void EnableControl ()
        {
            movement.enabled = true;
            shooting.enabled = true;

            canvasGameObject.SetActive (true);
        }


        // Used at the start of each round to put the tank into it's default state.
        public void Reset ()
        {
            instance.transform.position = m_SpawnPoint.position;
            instance.transform.rotation = m_SpawnPoint.rotation;

            instance.SetActive (false);
            instance.SetActive (true);
        }
    }
}