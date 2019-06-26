import AsymmetricEncrypter from './AsymmetricEncrypter'
import SymmetricEncrypter from './SymmetricEncrypter'

const HybridEncrypter = {

    encryptInput: async function (plainText, email) {

        var asymmetricpublicKey

        var plainSymmetricKey = SymmetricEncrypter.generateKey(16)

        var cipherText = SymmetricEncrypter.encryptInput(plainText, plainSymmetricKey)

        await AsymmetricEncrypter.loadPublicKey(email)
        .then(response => {
            asymmetricpublicKey = response
        })

        var cipherSymmetricKey = AsymmetricEncrypter.encryptInput(plainSymmetricKey, asymmetricpublicKey)

        var result = { 
            cipherText: cipherText, 
            cipherSymmetricKey : cipherSymmetricKey 
        };

        return result
    },
        

}
export default HybridEncrypter;
 