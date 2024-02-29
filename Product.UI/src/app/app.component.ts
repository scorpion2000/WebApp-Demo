import { Component } from '@angular/core';
import { Product } from './models/product';
import { ProductService } from './services/product.service';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrl: './app.component.css'
})
export class AppComponent {
  title = 'Product.UI';
  products: Product[] = [];
  productToEdit?: Product;

  constructor(private productService: ProductService) {}

  ngOnInit() : void {
    this.productService.getProducts().subscribe((result: Product[]) => (this.products = result));
  }

  updateProductList(products: Product[])
  {
    this.products = products;
  }

  initNewProduct()
  {
    this.productToEdit = new Product;
  }

  editProduct(product: Product)
  {
    this.productToEdit = product;
  }
}
