import * as React from "react";
import { useForm } from "react-hook-form";

type RegisterValues = {
  email: string;
  username: string;
  password: string;
};

export default function () {
  const { register, handleSubmit } = useForm<RegisterValues>();
  const onSubmit = (data: RegisterValues) => alert(JSON.stringify(data));

  return (
    <form onSubmit={handleSubmit(onSubmit)}>
      <div className="form-group">      
        <label>Email address</label>
        <input type="email" className="form-control" {...register("email")} placeholder="Email" />
      </div>
      <div className="form-group">      
        <label>Username</label>
        <input type="text" className="form-control" {...register("username")} placeholder="Username" />
      </div>
      <div className="form-group">
        <label>Password</label>
        <input type="password" className="form-control" {...register("password")} placeholder="Password" />
      </div>
      <div className="form-group m-3">
        <button  className="btn btn-primary px-5" type="submit">Register</button>
      </div>
    </form>
  );
}