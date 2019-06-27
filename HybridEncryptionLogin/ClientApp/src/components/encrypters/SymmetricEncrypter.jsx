import Config from '../Config'
const SymmetricEncrypter = {

   encryptInput: function(input, secretkey) {
         var CryptoJS         = window.CryptoJS

         var key              = CryptoJS.enc.Utf8.parse(secretkey);

         var iv               = CryptoJS.enc.Utf8.parse(Config.iv);

         var encrypted        = CryptoJS.AES.encrypt(CryptoJS.enc.Utf8.parse(input), key,
            {
               keySize: 128 / 8,
               iv: iv,
               mode: CryptoJS.mode.CBC,
               padding: CryptoJS.pad.Pkcs7
            });

         return encrypted.toString();
      },
      
   generateKey: function(length) {
         var result           = '';

         var characters       = 'ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789';

         var charactersLength = characters.length;

         for ( var i = 0; i < length; i++ ) {
            result += characters.charAt(Math.floor(Math.random() * charactersLength));
         }

         return result;
      }
}

export default SymmetricEncrypter;
 