public struct M_ButtonPressed
{
    private bool _status;

    public M_ButtonPressed(bool staatus)
    {
        _status = staatus;
    }

    public bool Status => _status;
}