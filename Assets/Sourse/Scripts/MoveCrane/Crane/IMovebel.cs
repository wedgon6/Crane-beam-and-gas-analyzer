using Cysharp.Threading.Tasks;

public interface IMovebel
{
    public UniTaskVoid Move(int direction);
    public void StopMove();
}