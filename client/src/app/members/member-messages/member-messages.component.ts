import { Component, Input, OnInit, ViewChild } from '@angular/core';
import { NgForm } from '@angular/forms';
import ImageLinks from 'src/app/constants/image-links';
import { Message } from 'src/app/_models/messages';
import { MessageService } from 'src/app/_services/message.service';

@Component({
  selector: 'app-member-messages',
  templateUrl: './member-messages.component.html',
  styleUrls: ['./member-messages.component.css']
})
export class MemberMessagesComponent implements OnInit {
  @ViewChild('messageForm') messageForm?: NgForm;
  @Input() username?: string;
  @Input() messages: Message[] = [];
  userLinks = ImageLinks.user;
  messageContent = '';

  constructor(private messageService: MessageService) { }

  ngOnInit(): void {
  }

 sendMessage(){
  if(!this.username) return;
  this.messageService.sendMessage(this.username, this.messageContent).subscribe({
    next: message => {
      this.messages.push(message);
      this.messageForm?.reset();
    }
  })
 }

}