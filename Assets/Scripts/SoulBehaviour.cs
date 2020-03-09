using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SoulBehaviour : MonoBehaviour
{
    protected Soul _soul;
    public virtual void PlayBehaviour(Soul soul) { _soul = soul; }
}
