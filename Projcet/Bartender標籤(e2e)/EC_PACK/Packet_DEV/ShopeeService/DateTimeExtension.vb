Module DateTimeExtension

    Private TimeStampBaseDay As DateTime = New DateTime(1970, 1, 1)

    Function GetTimeStampNow() As Long

        Dim seconds As Double = DateTime.UtcNow.Subtract(TimeStampBaseDay).TotalSeconds
        Return Convert.ToInt64(seconds)

    End Function

End Module