using System;

public abstract class Clonable<T> where T : Clonable<T>
{
    public abstract T DeepCopy();
    public virtual T ShallowCopy() => (T)this.MemberwiseClone();
}