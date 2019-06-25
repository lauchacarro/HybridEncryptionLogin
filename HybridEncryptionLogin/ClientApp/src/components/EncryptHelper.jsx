
const EncryptHelper = {

  publickey : "",

  setPublicKey: function (key){

      this.publickey = key
    
  },

  encryptInput: function (input) {
    const encrypt = new window.JSEncrypt();
    encrypt.setPublicKey(this.publickey);
    const result = encrypt.encrypt(input);
    return result;
  }
}

export default EncryptHelper;