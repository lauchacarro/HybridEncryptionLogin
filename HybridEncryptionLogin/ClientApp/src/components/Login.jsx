import React, { Component } from 'react';

import Form from 'react-bootstrap/Form'
    import Button from 'react-bootstrap/Button'


export class Login extends Component {




    // fetch('api/SampleData/WeatherForecasts')
    //   .then(response => response.json())
    //   .then(data => {
    //     this.setState({ forecasts: data, loading: false });
    //   });
  






  constructor(props) {
    super(props);

    
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
    fetch('/api/account/login', {
        method: 'POST',
        headers: {
            'Accept': 'application/json',
            'Content-Type': 'application/json',
        },
        body: JSON.stringify({
            email: this.state.email,
            password: this.state.password,
        })
        
        })
        .then(response => {
            if (response.ok) {
              alert("Inicio Sesión Correctamente")
            } else {
                alert("Email y/o Contraseña Incorrectos")
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
          <Form.Group controlId="email" bsSize="large">
            <Form.Label >Email</Form.Label >
            <Form.Control
              autoFocus
              type="email"
              value={this.state.email}
              onChange={this.handleChange}
            />
          </Form.Group>
          <Form.Group controlId="password" bsSize="large">
            <Form.Label >Password</Form.Label >
            <Form.Control
              value={this.state.password}
              onChange={this.handleChange}
              type="password"
            />
          </Form.Group>
          <Button
            block
            bsSize="large"
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
