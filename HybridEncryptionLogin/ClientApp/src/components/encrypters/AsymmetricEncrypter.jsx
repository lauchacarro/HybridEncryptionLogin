
const AsymmetricEncrypter = {


  loadPublicKey: async function (email) {
      const response  = await fetch('/api/keys/GetPublicKey?email=' + email)

      if (response.ok) {

          const json  = await response.json()

          return json["key"]
      }
      else {

        alert("Hubó Un Error En El Servidor")
      }
  },

  encryptInput: function(input, publicKey) {

        const encrypt = new window.JSEncrypt();

        encrypt.setPublicKey(publicKey);

        const result  = encrypt.encrypt(input);

        return result;
      }
}

export default AsymmetricEncrypter;