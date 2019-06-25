import React, { Component } from 'react';

import Form from 'react-bootstrap/Form'
import Button from 'react-bootstrap/Button'
import EncryptHelper from './EncryptHelper'


export class Login extends Component {

  constructor(props) {
    super(props);
    this.handleChange = this.handleChange.bind(this);
    this.handleSubmit = this.handleSubmit.bind(this);

  }

  state = {
    email: "",
    password: ""
  };

  validateForm() {
    return this.state.email.length > 0 && this.state.password.length > 0;
  }

  handleChange = event => {
    this.setState({
      [event.target.id]: event.target.value
    });
  }

  handleSubmit = event => {
    event.preventDefault();
    fetch('/api/keys/GetPublicKey?email=' + this.state.email, {
      method: 'GET'
      })
      .then(response => {
          if (response.ok) {
            
            response.json().then(json => {

              EncryptHelper.setPublicKey(json["key"])

              fetch('/api/account/login', {
                method: 'POST',
                headers: {
                    'Accept': 'application/json',
                    'Content-Type': 'application/json',
                },
                body: JSON.stringify({
                    email: this.state.email,
                    password: EncryptHelper.encryptInput(this.state.password),
                })
                
                })
                .then(response => {
                    if (response.ok) {
                      alert("Inicio Sesión Correctamente")
                    } else if (response.status == 400) {
                        alert("Email y/o Contraseña Incorrectos")
                    } else {
                      alert("Hubo Un Error En El Servidor")
                    }
                  })

              });


          } else {
              alert("Hubo Un Error En El Servidor")
          }
        })
  }


  render () {

    return (
        
        <div className="Login">
        <div className="alert alert-info">
                    Email: example@test.com<br />
                    Password: test
                </div>
        <Form onSubmit={this.handleSubmit}>
          <Form.Group controlId="email" >
            <Form.Label >Email</Form.Label >
            <Form.Control
              autoFocus
              type="email"
              value={this.state.email}
              onChange={this.handleChange}
            />
          </Form.Group>
          <Form.Group controlId="password" >
            <Form.Label >Password</Form.Label >
            <Form.Control
              value={this.state.password}
              onChange={this.handleChange}
              type="password"
            />
          </Form.Group>
          <Button
            block
            disabled={!this.validateForm()}
            type="submit"
          >
            Login
          </Button>
        </Form>
      </div>
    );
  }
}
