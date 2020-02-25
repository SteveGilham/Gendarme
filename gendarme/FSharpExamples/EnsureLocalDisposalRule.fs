namespace EnsureLocalDisposal

[<NoComparison>]
type StrongNameKeyData =
  { Blob : byte list
    Parameters : System.Security.Cryptography.RSAParameters }

  static member Make(data : byte array) =
    use csp = new System.Security.Cryptography.RSACryptoServiceProvider()
    csp.ImportCspBlob(data)
    let blob = csp.ExportCspBlob(true)
    { Blob = blob |> Array.toList
      Parameters = csp.ExportParameters(true) }