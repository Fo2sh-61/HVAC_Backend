function [best_params, best_rmse, convergence_history] = de_optimization( ...
    V_meas, I_meas, Vt_ref, N_diodes, lb, ub, opt_params)
%DE_OPTIMIZATION Compatibility wrapper for Differential_evolution.
%   Keeps the same interface used in existing optimization scripts.

    [best_params, best_rmse, convergence_history] = Differential_evolution( ...
        V_meas, I_meas, Vt_ref, N_diodes, lb, ub, opt_params);
end
