using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface Affectable {
    public void Burn(SpellSideEffectData sideEffectData);
    public void Poison(SpellSideEffectData sideEffectData);
    public void Freeze(SpellSideEffectData sideEffectData);
}
