import * as React from 'react';
import styles from './HomePagePendingWebPart.module.scss';
import { IHomePagePendingWebPartProps } from './IHomePagePendingWebPartProps';
import { escape } from '@microsoft/sp-lodash-subset';
import { Link } from 'office-ui-fabric-react/lib/Link';
import { CommandBar } from 'office-ui-fabric-react/lib/CommandBar';
import pnp from "sp-pnp-js";
import { Web } from "sp-pnp-js";
import { Item, Items } from 'sp-pnp-js/lib/sharepoint/items';
import { autobind } from 'office-ui-fabric-react/lib/Utilities';
import { Label } from 'office-ui-fabric-react';

let _items: {
  key: string,
  name: string,
  value: number,
  Title: string,
}[] = [];


export default class HomePagePendingWebPart extends React.Component<IHomePagePendingWebPartProps, { items: { key: string, name: string }[]; }> {
  public _isSuperUserOrApprover: boolean = false;
  constructor(props: {}) {
    super(props);

    pnp.sp.web.currentUser.get().then(function (res) {
      pnp.sp.web.lists.getByTitle('CategoryMaster').items.orderBy("ID", false).filter("Category_x0020_Head eq " + res.Id).get().then(
        response => {
          response.map(item => {
            _items.push({ key: item.ID, name: item.Title, value: item.ID, Title: item.Title })
          }
          )
        }
      )
      if (_items.length === 0) {
        pnp.sp.web.siteGroups.getByName("Super User").users.get().then(function (usersList) {
          for (var i = 0; i < usersList.length; i++) {
            console.log("Title: " + usersList[i].Title);
            console.log("ID: " + usersList[i].Id);
            if (usersList[i].Title == res.Title) {
              _items.push({ key: "test", name: "Name", value: 5, Title: "Title" });
            }
          }
        })
      }
      return _items;

    })
    this.state = {
      items: _items
    };
  }

  public setSuperUser() {
    pnp.sp.web.currentUser.get().then(function (res) {
      pnp.sp.web.lists.getByTitle('CategoryMaster').items.orderBy("ID", false).filter("Category_x0020_Head eq " + res.Id).get().then(
        response => {
          response.map(item => {
            _items.push({ key: item.ID, name: item.Title, value: item.ID, Title: item.Title })
            return _items;
          }
          )
        }
      )
    })
  }
  public render(): React.ReactElement<IHomePagePendingWebPartProps> {

    const linkStyle = {


    }

    if (this.state.items.length === 0) {
      console.log(this.state.items.length)
      setTimeout(() => { this.setState({ items: _items }) }, 500)
      return null
    }

    let { items } = this.state

    return (
      <div className="ms-Grid-col ms-u-sm12 ms-u-md12 ms-u-lg12" style={{ border: "1px solid #038387" }}>
        <a style={{ textDecoration: 'none' }} href="https://bajajelect.sharepoint.com/teams/ConnectApp/SitePages/Pending Approval Requests.aspx">
          <Label className="ms-fontSize-xxl" style={{ cursor: 'pointer' }}><i alt="My Pending Request" className="ms-Icon ms-Icon--AlarmClock" aria-hidden="true"></i> Connect Call Pending For Your Action</Label>
        </a>

      </div>
    );
  }
}
