import React, { Component } from 'react';
import Lorem from './Lorem';
import _ from 'lodash';
import './App.css';

class App extends Component {
  initButtons(el) {
    const rect = el.getBoundingClientRect();
    console.log('Bounding rect for buttons:', rect);
    el.style.position = 'fixed';
    el.style.top = `${rect.top}px`;
    el.style.width = `${rect.width}px`;
  }

  onScroll(e) {
    console.log('Got onScroll event. Scroll top: ', this.container.scrollTop);

    window.requestAnimationFrame(() => {
      this.buttons.style.transform = `translateY(${this.container.scrollTop}px)`;
    });
  }

  render() {
    return (
      // <div className="app" onScroll={_.throttle(e => this.onScroll(e), 20)} ref={el => this.container = el}>
      <div className="app" ref={el => this.container = el}>
        <div className="container">
          <div className="left">
            <Lorem />
          </div>
          <div className="right">
            <div className="buttons" ref={el => this.initButtons(el)}>
              <button type="button">Button 1</button>
              <button type="button">Button 1</button>
            </div>
          </div>
        </div>
      </div>

    );
  }
}

export default App;
