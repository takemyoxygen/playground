import React, { Component } from 'react';
import {Link, Router, Route, HashRouter} from 'react-router-dom';
import {createHashHistory} from 'history';
import createAppHistory from './history';

const baseHistory = createHashHistory({ hashType: 'noslash' });

const Home = () => (
  <div>
    <h1>Home</h1>
    <ul>
      <li><Link to="/item/1">Item 1</Link></li>
      <li><Link to="/item/2">Item 2</Link></li>
      <li><Link to="/item/3">Item 3</Link></li>
    </ul>
  </div>
)

const Item = props => (
  <div>
    <h1>Item {props.match.params.id}</h1>
  </div>
)

const A = () => (
  <div>
    <h1>A</h1>
  </div>
)

const B = () => (
  <div>
    <h1>B</h1>
  </div>
)

class App extends Component {
  render() {
    return (
      <div >
        <Router history={createAppHistory(baseHistory, 'page')}>
          <div>
            <Route exact path="/" component={Home} />
            <Route path="/item/:id" component={Item} />
            <Route path="/a" component={A}/>
            <Route path="/b" component={B}/>
          </div>
        </Router>

        {/* <HashRouter >
          <div>
            <Route path="/a" component={A}/>
            <Route path="/b" component={B}/>
          </div>
        </HashRouter> */}

      </div>
    );
  }
}

export default App;
