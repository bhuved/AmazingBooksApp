import { CommonModule } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { Order } from '../../model/order';
import { OrderApiService } from '../../services/order-api.service';
import { ActivatedRoute } from '@angular/router';

@Component({
  selector: 'app-order-details',
  imports: [CommonModule],
  templateUrl: './order-details.component.html',
  styleUrl: './order-details.component.css',
})
export class OrderDetailsComponent implements OnInit {
  order: Order | undefined;
  errorMessage = '';
  message = '';
  isSuccess = '';
  isCancel = '';

  constructor(
    private orderApi: OrderApiService,
    private route: ActivatedRoute
  ) {}

  ngOnInit(): void {
    this.route.params.subscribe((param) => {
      let id = param['id'];
      if (id == null) {
        this.errorMessage = 'Order information missing';
      } else {
        this.orderApi.GetOrderDetails(id).subscribe({
          next: (data: any) => {
            this.order = data;

            this.route.queryParams.subscribe((params) => {
              this.isSuccess = params['success'];
              this.isCancel = params['canceled'];
              if (this.isSuccess) {
                this.order!.status = 'Placed';
                this.order!.paymentMethod = 'Online';
                this.order!.paymentStatus = 'Paid';
                this.orderApi.UpdateOrderStatus(this.order!).subscribe({
                  next: (data) => {
                    this.message = 'Payment done, Order placed successfully';
                  },
                  error: (error) => {
                    console.log(error);
                  },
                });
              } else if (this.isCancel) {
                console.log('Cancel: ' + this.isCancel);
              }
            });
          },
          error: (error) => {
            this.errorMessage = 'Error fetching error information';
            console.log(error);
          },
        });
      }
    });
  }

  SaveOrderStatus() {
    this.order!.status = 'Cancelled';
    this.order!.cancellationDate = new Date();
    this.order!.paymentStatus = 'Cancelled';
    this.orderApi.UpdateOrderStatus(this.order!).subscribe({
      next: (data) => {
        this.message = 'Order cancelled';
      },
      error: (error) => {
        this.errorMessage = 'Error cancelling Order';
        console.log(error);
      },
    });
  }

  Clear() {
    this.errorMessage = '';
    this.message = '';
  }
}
