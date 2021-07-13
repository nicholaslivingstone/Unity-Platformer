using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MultiAnimator
{
    private List<Animator> animators = new List<Animator>();
    private Dictionary<string, List<Animator>> animatorDictionary = new Dictionary<string, List<Animator>>(); 

    // Start is called before the first frame update
    public MultiAnimator(GameObject gameObject)
    {
        // Check for & add animator on gameObject
        Animator primaryAnimator = gameObject.GetComponent<Animator>();
        if(primaryAnimator) AddAnimator(primaryAnimator); 

        // Add animators on children
        foreach(Animator childAnimator in gameObject.GetComponentsInChildren<Animator>()){
            AddAnimator(childAnimator);
        }
    }

    /// <summary>
    /// Adds animator and it's parameters to the multiAnimator for future use
    /// </summary>
    /// <param name="animator"> Animator to add </param>
    private void AddAnimator(Animator animator){
        string parameterName;

        animators.Add(animator);

        foreach(AnimatorControllerParameter parameter in animator.parameters){
            parameterName = parameter.name; 

            // Animator contains a parameter we haven't seen before
            // Setup the list for this parameter before adding to it
            if(!animatorDictionary.ContainsKey(parameterName)){ 
                animatorDictionary.Add(parameterName, new List<Animator>());
            }

            animatorDictionary[parameterName].Add(animator);
        }
    }

    public void SetFloat(string name, float value){
        foreach(Animator animator in animatorDictionary[name]){
            animator.SetFloat(name, value);
        }
    }

    public void SetFloat(string name, float value, float dampTime, float deltaTime){
        foreach(Animator animator in animatorDictionary[name]){
            animator.SetFloat(name, value, dampTime, deltaTime);
        }
    }

    public void SetBool(string name, bool value){
        foreach(Animator animator in animatorDictionary[name]){
            animator.SetBool(name, value);
        }
    }

    public void SetInteger(string name, int value){
        foreach(Animator animator in animatorDictionary[name]){
            animator.SetInteger(name, value);
        }
    }

    public void SetTrigger(string name){
        foreach(Animator animator in animatorDictionary[name]){
            animator.SetTrigger(name);
        }
    }
}
