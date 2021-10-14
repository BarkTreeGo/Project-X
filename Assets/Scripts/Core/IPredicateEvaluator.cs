using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameDevTV.Utils
{
    public interface IPredicateEvaluator 
    {
        public bool? Evaluate(string predicate, string[] parameters); //the ? means the bolean can be true, false or null        
    }
}
