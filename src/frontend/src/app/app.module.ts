import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';


import { AppComponent } from './app.component';
import {ChessboardModule} from 'ng2-chessboard';


@NgModule({
  declarations: [
    AppComponent
  ],
  imports: [
    BrowserModule,
    ChessboardModule
  ],
  providers: [],
  bootstrap: [AppComponent]
})
export class AppModule { }
