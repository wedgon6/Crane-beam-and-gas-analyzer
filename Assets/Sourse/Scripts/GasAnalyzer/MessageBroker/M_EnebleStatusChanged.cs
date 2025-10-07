public struct M_EnebleStatusChanged
{
    private bool _isEneble;

    public M_EnebleStatusChanged(bool isEneble)
    {
        _isEneble = isEneble;
    }

    public bool IsEneble => _isEneble;
}