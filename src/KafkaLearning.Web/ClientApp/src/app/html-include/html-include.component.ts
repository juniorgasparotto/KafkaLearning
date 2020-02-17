import { Component, Input, OnInit } from "@angular/core";
import { HttpClient } from "@angular/common/http";

@Component({
  selector: 'html-include',
  template: '<div [innerHtml]="html">waiting...</div>'
})
export class HtmlIncludeComponent implements OnInit {
  public html: string = "";

  @Input()
  public set htmlUrl(value: string) {
    this.http.get(value, { responseType: 'text' })
      .subscribe(
        data => {
          this.html = data;
        },
        error => {
          this.html = JSON.stringify(error);
        }
      );
  }

  constructor(private http: HttpClient) {

  }

  ngOnInit(): void {

  }
}