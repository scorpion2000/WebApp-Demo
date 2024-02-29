import { Injectable } from '@angular/core';
import { Product } from '../models/product';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment';

@Injectable({
  providedIn: 'root'
})
export class ProductService {
  private url = "Product";

  constructor(private http: HttpClient) { }

  //Oké, nem tudom miért, de az Angular néha cseszik betenni a body részt a linkbe
  //Például a törlésnél, console-ban látom a product-id-t, de a hibás linkben már nincs benne

  public getProducts() : Observable<Product[]> {
    return this.http.get<Product[]>(environment.apiUrl + '/' + this.url);
  }

  public updateProduct(product: Product) : Observable<Product[]> {
    console.log(product.id);
    return this.http.put<Product[]>(environment.apiUrl + '/' + this.url, product);
  }

  public createProduct(product: Product) : Observable<Product[]> {
    console.log(product.id);
    return this.http.post<Product[]>(environment.apiUrl + '/' + this.url, product);
  }

  public deleteProduct(product: Product) : Observable<Product[]> {
    console.log(product.id);
    return this.http.delete<Product[]>(environment.apiUrl + '/' + this.url + '/' + product.id);
  }
}
