namespace DoNotThrowInNonCatchClauses

open System.Security
open System.Security.Cryptography

module Example =
    let transformCryptographicException f =
      try
        f ()
      with
      | :? CryptographicException as c -> raise ((c.Message, c) |> SecurityException)