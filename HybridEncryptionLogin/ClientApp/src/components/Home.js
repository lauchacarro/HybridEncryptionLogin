import React, { Component } from 'react';

export class Home extends Component {
  static displayName = Home.name;

  render () {
    return (
      <div>
        <h1>HybridEncryptionLogin</h1>
        <p>Es una simple prueba de concepto para mostrar como utilizar la encriptación hibrida (AES + RSA) para enviar información sensible desde el lado de cliente (<a href='https://facebook.github.io/react/'>React</a>) al servidor (<a href='https://get.asp.net/'>ASP.NET Core</a>).</p>
         
        <h2>Criptografía</h2>
        <p>En criptografía, los algoritmos de cifrado generan claves en forma de series de bits que se emplean para encriptar y desencriptar fragmentos de información. La forma en que dichas claves se utilizan da cuenta de la diferencia entre encriptación simétrica y asimétrica. </p>
        <h3>Criptografía simétrica</h3>
        <p>Los algoritmos de encriptación simétrica utilizan la misma llave para llevar a cabo las funciones de encriptación y desencriptación. Algunos algoritmos de encriptación simétrica son AES y DES.</p>
        <h3>Criptografía asimétrica</h3>
        <p> En los sistemas asimétricos, la clave utilizada para la encriptación se conoce como “clave pública” y puede ser compartida libremente con el resto de la gente. Por el contrario, la clave empleada para la desencriptación, denominada “clave privada”, debe ser guardada en secreto. Algunos algoritmos de encriptación asimétrica son RSA y DSA.</p>
        <h3>Criptografía híbrida</h3>
        <p>La criptografía híbrida es un método criptográfico que usa tanto un cifrado simétrico como un asimétrico. 
          En esta aplicación se generan las claves públicas y privadas en el lado del servidor. 
          La clave pública es enviada al cliente mientras la privada se guarda en el servidor. 
          Del lado de cliente, en cada ocación que se necesita enviar una petición al servidor se genera una nueva clave simétrica de manera aleatoria para encriptar con AES la información sencible 
          y finalizado esto se encripta la clave simétrica con la clave pública. 
          En el momento en que se envia la información sensible ya encriptada, a la vez se envia la clave simétrica también encriptada. 
          De esta manera en el servidor con la clave privada guardada desencripta la clave simétrica para desencriptar la informácion sensible. 
         </p>
         <p>En esta aplicación al realizar el <a href="login">Login</a>, antes de enviar las credenciales, se pide una nueva clave pública y la 
         nueva clave privada se almacena en cache del servidor. Una vez obtenida la clave pública en el lado del cliente se realiza el proceso 
         encriptación híbrida y se envia al servicio el email del usuario en texto plano, la contraseña encriptada simétricamente y la clave simétrica que fue encriptada 
         asimétricamente se envia en un header. </p>
        
      </div>
    );
  }
}
